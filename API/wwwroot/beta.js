const canvas = document.querySelector('canvas');
const ctx = canvas.getContext('2d');

function render(time) {
  canvas.width = 720; 
  canvas.height = 480; 

  var imgData = ctx.createImageData(480, 480);

  var idPtr = 0;

  for(var yPos = 0; yPos < 24 * 20; yPos++)
  {
    for(var xPos = 0; xPos < 40 * 12; xPos++)
    {
        imgData.data[idPtr] = charArray[((12 * yPos) % 240) + (xPos % 12)] == 1 ? 220 : 0;
        imgData.data[idPtr+1] = charArray[((12 * yPos) % 240) + (xPos % 12)] == 1 ? 220 : 0;
        imgData.data[idPtr+2] = charArray[((12 * yPos) % 240) + (xPos % 12)] == 1 ? 0 : 0;
        imgData.data[idPtr+3] = 255;
        idPtr = idPtr + 4;
    }
  }

  ctx.putImageData(imgData, 0, 0);
	ctx.scale(3, 2);
  
  requestAnimationFrame(render);
}
requestAnimationFrame(render);

function makeChars() {
    return new Uint8Array([
        // 'a'
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0,
        0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
        0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1,
        0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    ]);
}

var charArray = makeChars();
