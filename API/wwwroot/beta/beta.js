
(function () {

  // Page initialisation
  $(window).load(function () {

  const canvas = document.querySelector('canvas');
  const ctx = canvas.getContext('2d');

  function render(time) {
  canvas.width = 480; 
  canvas.height = 480;

  var imgData = ctx.createImageData(480, 480);
  var charPos = 0;

  
  for(var yCol = 0; yCol < 24; yCol++)
  {
    // New row, reset flags and colours
    var backCol = 0;
    var foreCol = 7;
    var currentCharset = charSmoothed;
  
    for(var xCol = 0; xCol < 40; xCol++)
    {
      idPtr = (xCol * 4 * 12) + (yCol * 40 * 4 * 12 * 20);

      // Process a new control code
      var charCode = testPage[charPos++];
      if(charCode < 32)
      {
        switch(charCode)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
              foreCol = charCode;
              break;

            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
              foreCol = charCode - 16;
              if(currentCharset == charSmoothed) { currentCharset = charGraphics };
              break;

            case 25:
              currentCharset = charGraphics;
              break;
            case 26:
              currentCharset = charSeparated;
              break;
            case 28:
              backCol = 0;
              break;
            case 29:
              backCol = foreCol;
              break;

            default:
              break;

        }


        charNo = 32;
      }
    
      
      // Displayable character, map to character definitions
      var charNo = charCode - 32;

      for(var yPos = 0; yPos < 20; yPos++)
      {
        for(var xPos = 0; xPos < 12; xPos++)
        {
          imgData.data[idPtr] = currentCharset[(charNo * 240) + (12 * yPos) + xPos] == 1 ? getRGB_Red(foreCol) : getRGB_Red(backCol);
          imgData.data[idPtr+1] = currentCharset[(charNo * 240) + (12 * yPos) + xPos] == 1 ? getRGB_Green(foreCol) : getRGB_Green(backCol);
          imgData.data[idPtr+2] = currentCharset[(charNo * 240) + (12 * yPos) + xPos] == 1 ? getRGB_Blue(foreCol) : getRGB_Blue(backCol);
          imgData.data[idPtr+3] = 255;
          idPtr = idPtr + 4;
        }
        idPtr = idPtr + (4 * 39 * 12);
      }
    }
  }

  ctx.scale(2, 3);
  ctx.putImageData(imgData, 0, 0);

  requestAnimationFrame(render);
  }
  

  var charSmoothed = makeSmoothedChars(getChars());
  var charGraphics = makeGraphicChars(getChars(), false);
  var charSeparated = makeGraphicChars(getChars(), true);

  var testPage = getTestPage();

  requestAnimationFrame(render);

  });
  })();