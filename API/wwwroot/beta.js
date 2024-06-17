



const canvas = document.querySelector('canvas');
const ctx = canvas.getContext('2d');
let width = 300;
let height = 150;

const observer = new ResizeObserver((entries) => {
  width = canvas.clientWidth;
  height = canvas.clientHeight;
});
observer.observe(canvas);

// not import but draw something just to showcase

function render(time) {
  canvas.width = width;
  canvas.height = height;

  ctx.save();
  
  ctx.beginPath();
  ctx.strokeStyle = '#000000';
  ctx.lineWidth = 10;
  ctx.rect(20, 20, width-40, height-40);
  ctx.stroke();

  ctx.beginPath();
  ctx.strokeStyle = '#ff0000';
  ctx.lineWidth = 10;
  ctx.rect(50, 50, width-100, height-100);
  ctx.stroke();
  ctx.restore();

  requestAnimationFrame(render);
}
requestAnimationFrame(render);
