
(function () {

  // Page initialisation
  $(window).load(function () {

  const canvas = document.querySelector('canvas');
  const ctx = canvas.getContext('2d');
  canvas.width = 480; 
  canvas.height = 480;

  var prevCol = bgCol = holdChar = 0;
  var col = 7;
  var sep = holdOff = gfx = heldChar = false;
  var dbl = oldDbl = secondHalfOfDouble = wasDbl = false;
  var lastRefresh = false;
  var newPage = 1;
  var pageCycle = 100;

  var charSmoothed = makeSmoothedChars(getChars());
  var charGraphics = makeGraphicChars(getChars(), false);
  var charSeparated = makeGraphicChars(getChars(), true);
  var testPage = getTestPage();

  function render(time) {
    if(!lastRefresh || newPage != 24 || time - lastRefresh >= 200) {
        lastRefresh = time;
        newPage = newPage >= 24 ? 24 : newPage+2;
            
        var imgData = ctx.createImageData(480, 480);
        insertPageHeader(testPage, pageCycle);
        
        if(newPage == 24)
        {
            pageCycle = (pageCycle > 190) ? 100 : pageCycle + ((Math.floor((Math.random() * 10)) == 1) ? 2 : 1);
        }

        var charPos = 0;
     
        // Render page
        for(var yCol = 0; yCol < newPage; yCol++)
        {
            col = 7;
            bg = 0;
            holdChar = false;
            heldChar = 0x20;
            nextGlyphs = heldGlyphs = charSmoothed;
            sep = gfx = dbl = false;
            
            if (secondHalfOfDouble) {
                secondHalfOfDouble = false;
            } else {
                secondHalfOfDouble = wasDbl;
            }
            wasDbl = false;

            for(var xCol = 0; xCol < 40; xCol++)
            {
                idPtr = (xCol * 4 * 12) + (yCol * 40 * 4 * 12 * 20);

                oldDbl = dbl;
                prevCol = col;
                curGlyphs = nextGlyphs;
                data = testPage[charPos++];

                if (data < 0x20) {
                    data = handleControlCode(data);
                } else if (gfx) {
                    heldChar = data;
                    heldGlyphs = curGlyphs;
                }

                // Displayable character, map to character definitions
                var charDef = (secondHalfOfDouble && !dbl) ? 0 : data - 32;
                
                for(var yPos = 0; yPos < 20; yPos++)
                {
                    actualY = dbl ? Math.floor(yPos / 2) + (secondHalfOfDouble ? 10 : 0) : yPos;
                    for(var xPos = 0; xPos < 12; xPos++)
                    {
                        let setPixel = curGlyphs[(charDef * 240) + (12 * actualY) + xPos] == 1;
                        imgData.data[idPtr] = setPixel ? getRGB_Red(prevCol) : getRGB_Red(bg);
                        imgData.data[idPtr+1] = setPixel ? getRGB_Green(prevCol) : getRGB_Green(bg);
                        imgData.data[idPtr+2] = setPixel ? getRGB_Blue(prevCol) : getRGB_Blue(bg);
                        imgData.data[idPtr+3] = setPixel || (!setPixel && bg != 0) ? 255 : 0;
                        idPtr = idPtr + 4;
                    }
                    
                    idPtr = idPtr + (4 * 39 * 12);      
                }
                
                if (holdOff) {
                    holdChar = false;
                    heldChar = 32;
                }
            }
        }

        ctx.scale(2, 3);
        ctx.putImageData(imgData, 0, 0);
    }
    
    requestAnimationFrame(render);
  }
  

  function setNextChars()
  {
    if (gfx) {
        if (sep) {
            nextGlyphs = charSeparated;
        } else {
            nextGlyphs = charGraphics;
        }
    } else {
        nextGlyphs = charSmoothed;
    }
  }

  function handleControlCode(data)
  {
    holdOff = false;
    switch (data) {
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
            gfx = false;
            col = data;
            setNextChars();
            break;

        case 12:
        case 13:
            dbl = !!(data & 1);
            if (dbl) wasDbl = true;
            break;

        case 17:
        case 18:
        case 19:
        case 20:
        case 21:
        case 22:
        case 23:
            gfx = true;
            col = data & 7;
            setNextChars();
            break;
        case 24:
            col = prevCol = bg;
            break;
        case 25:
            sep = false;
            setNextChars();
            break;
        case 26:
            sep = true;
            setNextChars();
            break;
        case 28:
            bg = 0;
            break;
        case 29:
            bg = col;
            break;
        case 30:
            holdChar = true;
            break;
        case 31:
            holdOff = true;
            break;
        }

      if (holdChar && dbl === oldDbl) {
          data = heldChar;
          if (data >= 0x40 && data < 0x60) data = 0x20;
          curGlyphs = heldGlyphs;
      } else {
          heldChar = 0x20;
          data = 0x20;
      }
   
      return data;
  }

  // Start the emulation
  requestAnimationFrame(render);

  });
  })();