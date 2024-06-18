



const canvas = document.querySelector('canvas');
const ctx = canvas.getContext('2d');
let width = 300;
let height = 150;

const observer = new ResizeObserver((entries) => {
  width = canvas.clientWidth;
  height = canvas.clientHeight;
});
observer.observe(canvas);


function render(time) {
  canvas.width = 480; //width;
  canvas.height = 480; //height;

  var imgData = ctx.createImageData(480, 480);

  var idPtr = 0;

  for(var yPos = 0; yPos < 24 * 20; yPos++)
  {
    for(var xPos = 0; xPos < 40 * 12; xPos++)
    {
        imgData.data[idPtr] = charArray[((12 * yPos) % 240) + (xPos % 12)] == 1 ? 255 : 0;
        imgData.data[idPtr+1] = charArray[((12 * yPos) % 240) + (xPos % 12)] == 1 ? 255 : 0;
        imgData.data[idPtr+2] = charArray[((12 * yPos) % 240) + (xPos % 12)] == 1 ? 255 : 0;
        imgData.data[idPtr+3] = 255;
        idPtr = idPtr + 4;
    }
  }

  ctx.putImageData(imgData, 0, 0);

  /*
  var startX = 50;
  var startY = 50;
  var posX = startX;
  var posY = startY;

  for (var charPos = 0; charPos < 240; charPos++)
  {
    if(charPos % 12 == 0)
    {
      posY++;
      posX = startX;
    }
     

    if(charArray[charPos] == 1)
    {
      ctx.save();
      ctx.beginPath();
      ctx.strokeStyle = '#f0f0f0';
      ctx.lineWidth = 1;
      ctx.rect(posX, posY, 1, 1);
      ctx.stroke();
      ctx.restore();
      posX++;
    }

    if(charArray[charPos] == 0)
    {
      /*
      ctx.save();
      ctx.beginPath();
      ctx.strokeStyle = '#0000f0';
      ctx.lineWidth = 1;
      ctx.rect(posX, posY, 1, 1);
      ctx.stroke();
      ctx.restore();
   
  */
  
  

  
  requestAnimationFrame(render);
}
requestAnimationFrame(render);

function makeChars() {
    // prettier-ignore
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
