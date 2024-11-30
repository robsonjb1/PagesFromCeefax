(function () {
    // Page initialisation
    $(window).on("load", function () {

    // Canvas
    const canvas = document.querySelector('canvas');
    canvas.width = 718; 
    canvas.height = 560;
    
    const ctx = canvas.getContext('2d', { willReadFrequently: true });
    var video = document.createElement("video");
    video.src = "media/4-13.m4v";
      
    // Functions
    window.addEventListener('click', function(e) {
        video.currentTime = 60;
        video.play();  // start playing
        timerTrigger(); //Start rendering
    });

    function timerTrigger(time) {
        ctx.drawImage(video,0,0,canvas.width, canvas.height);

        var imageData = ctx.getImageData(0,0,canvas.width,canvas.height);
     
       /* var cPtr = 0;
        for(var yCol = 0; yCol < canvas.height; yCol++)
        {
            for(var xCol = 0; xCol < canvas.width; xCol++)
            {
                if(
                    imageData.data[cPtr] < 5 &&
                    imageData.data[cPtr+1] < 5 &&
                    imageData.data[cPtr+2] < 5)
                {
                    imageData.data[cPtr+3] = 0;
                }

                cPtr+= 4;
            }
        }
*/
        ctx.putImageData(imageData, 0, 0);


        requestAnimationFrame(timerTrigger); // wait for the browser to be ready to present another animation fram.       
    }

    // Start the emulation
    requestAnimationFrame(timerTrigger);
  });
  })();