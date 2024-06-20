
(function () {

  // Page initialisation
   $(window).load(function () {

    const canvas = document.querySelector('canvas');
    const ctx = canvas.getContext('2d');

    function render(time) {
    canvas.width = 480; 
    canvas.height = 480;

    var imgData = ctx.createImageData(480, 480);
    var charNo = 0;

    for(var yCol = 0; yCol < 24; yCol++)
    {
      for(var xCol = 0; xCol < 40; xCol++)
      {
        idPtr = (xCol * 4 * 12) + (yCol * 40 * 4 * 12 * 20);

        for(var yPos = 0; yPos < 20; yPos++)
        {
          for(var xPos = 0; xPos < 12; xPos++)
          {
            imgData.data[idPtr] = charSmoothed[(charNo * 240) + (12 * yPos) + xPos] == 1 ? 220 : 0;
            imgData.data[idPtr+1] = charSmoothed[(charNo * 240) + (12 * yPos) + xPos] == 1 ? 220 : 0;
            imgData.data[idPtr+2] = charSmoothed[(charNo * 240) + (12 * yPos) + xPos] == 1 ? 220 : 0;
            imgData.data[idPtr+3] = 255;
            idPtr = idPtr + 4;
          }
          idPtr = idPtr + (4 * 39 * 12);
        }

        charNo++;
        if(charNo > 95)
        {
          charNo = 0;
        }
      }
    }

    ctx.scale(2, 3);
    ctx.putImageData(imgData, 0, 0);
    
    requestAnimationFrame(render);
    }
    requestAnimationFrame(render);
  
    var charSmoothed = makeSmoothedChars(makeChars());  

    });
  })();