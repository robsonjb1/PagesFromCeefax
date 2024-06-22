
(function () {

  // Page initialisation
  $(window).load(function () {

  const canvas = document.querySelector('canvas');
  const ctx = canvas.getContext('2d');

  prevCol = bgCol = holdChar = 0;
  col = 7;
  sep = holdOff = gfx = heldChar = false;
  dbl = oldDbl = secondHalfOfDouble = wasDbl = false;
  flash = flashOn = false;
  flashTime = 0;

  var charSmoothed = makeSmoothedChars(getChars());
  var charGraphics = makeGraphicChars(getChars(), false);
  var charSeparated = makeGraphicChars(getChars(), true);
  var testPage = getTestPage();

  function render(time) {
    canvas.width = 480; 
    canvas.height = 480;

    var imgData = ctx.createImageData(480, 480);
    insertPageHeader(testPage);

    var charPos = 0;
    if (++flashTime === 64) flashTime = 0;
    flashOn = flashTime < 16;

    // Render page
    for(var yCol = 0; yCol < 24; yCol++)
    {
        col = 7;
        bg = 0;
        holdChar = false;
        heldChar = 0x20;
        nextGlyphs = heldGlyphs = charSmoothed;
        flash = sep = gfx = dbl = false;
        
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
            var charDef = (flash && flashOn) || (secondHalfOfDouble && !dbl) ? 0 : data - 32;
            
            for(var yPos = 0; yPos < 20; yPos++)
            {
                actualY = dbl ? Math.floor(yPos / 2) + (secondHalfOfDouble ? 10 : 0) : yPos;
                for(var xPos = 0; xPos < 12; xPos++)
                {
                    imgData.data[idPtr] = curGlyphs[(charDef * 240) + (12 * actualY) + xPos] == 1 ? getRGB_Red(prevCol) : getRGB_Red(bg);
                    imgData.data[idPtr+1] = curGlyphs[(charDef * 240) + (12 * actualY) + xPos] == 1 ? getRGB_Green(prevCol) : getRGB_Green(bg);
                    imgData.data[idPtr+2] = curGlyphs[(charDef * 240) + (12 * actualY) + xPos] == 1 ? getRGB_Blue(prevCol) : getRGB_Blue(bg);
                    imgData.data[idPtr+3] = 255;
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

        case 8:
            flash = true;
            break;
        case 9:
            flash = false;
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