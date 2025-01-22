

var canvas = document.getElementById("teletextCanvas"); // get the canvas from the page

canvas.width = 500; 
canvas.height = 500;

var ctx = canvas.getContext("2d", { willReadFrequently: true });

var videoContainer; // object to hold video and associated info
var video = document.createElement("video"); // create a video element

// the video will now begin to load.
// As some additional info is needed we will place the video in a
// containing object for convenience
video.autoPlay = false; // ensure that the video does not auto play
video.loop = true; // set the video to loop.
videoContainer = {  // we will add properties as needed
     video : video,
     ready : false,   
};


let episodeDurations = [
    3066.32,
    2916.08,
    3110.48,
    3079.28,
    2901.20
];

let episodeUrls = [
    "https://api.onedrive.com/v1.0/shares/u!aHR0cHM6Ly8xZHJ2Lm1zL3YvcyFBdVQyQVlXUWRGaHJ2dFJydU41bkFkbG9hekFZZ2c_ZT1VZXAxSGM/root/content",
    "https://api.onedrive.com/v1.0/shares/u!aHR0cHM6Ly8xZHJ2Lm1zL3YvcyFBdVQyQVlXUWRGaHJ2dFJYdEVZOW1PdWEzSHpQeUE_ZT05YTZNc1Y/root/content",
    "https://api.onedrive.com/v1.0/shares/u!aHR0cHM6Ly8xZHJ2Lm1zL3YvcyFBdVQyQVlXUWRGaHJ2dFJlX3FaQWd4Vm9kdDRZbWc_ZT1SYzJQdDM/root/content",
    "https://api.onedrive.com/v1.0/shares/u!aHR0cHM6Ly8xZHJ2Lm1zL3YvcyFBdVQyQVlXUWRGaHJ2dFJjRDhfREdOZWxrSGVDWHc_ZT13NUZkWmY/root/content",
    "https://api.onedrive.com/v1.0/shares/u!aHR0cHM6Ly8xZHJ2Lm1zL3YvcyFBdVQyQVlXUWRGaHJ2dFJiUTY5YzBsV2hYTEFMMEE_ZT1NUWZkdW0/root/content"
]

let now = new Date();
const totalTimes = episodeDurations.reduce((partialSum, a) => partialSum + a, 0);

// Time into day
let dayPosition = Math.floor(((now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) % totalTimes);
let currentEpisode = 0;

// Find out which episode this is
var sum = 0;
for (let i=0; i<episodeDurations.length; i++)
{
    if(sum+episodeDurations[i] > dayPosition)
    {
        currentEpisode = i;
        video.currentTime = dayPosition - sum;
        console.log('Moving to episode ' + currentEpisode + " position " + video.currentTime);
        break;
    }
    else
    {
        sum += episodeDurations[i];
    }
}

video.src = episodeUrls[currentEpisode];

video.src = "https://api.onedrive.com/v1.0/shares/u!aHR0cHM6Ly8xZHJ2Lm1zL3YvcyFBdVQyQVlXUWRGaHJ2dFJiUTY5YzBsV2hYTEFMMEE_ZT1NUWZkdW0/root/content";

// https://learn.microsoft.com/en-us/graph/api/shares-get?view=graph-rest-1.0&tabs=http#encoding-sharing-urls


video.oncanplay = readyToPlayVideo; // set the event to the play function that 
                                  // can be found below
function readyToPlayVideo(event){ // this is a referance to the video
    // the video may not match the canvas size so find a scale to fit
    videoContainer.scale = Math.min(
                         canvas.width / this.videoWidth, 
                         canvas.height / this.videoHeight); 
    videoContainer.ready = true;
    // the video can be played so hand it off to the display function
    requestAnimationFrame(updateCanvas);
}

function updateCanvas(){
    console.log(video.duration);

    if(video.currentTime >= video.duration - 1) {
        // Next episode
        video.currentTime = 0;
        currentEpisode++;
        if(currentEpisode == episodeDurations.length)
        {
            currentEpisode = 0;
        }
        console.log('Moving to episode ' + currentEpisode + " position 0");
        video.src = episodeUrls[currentEpisode];
        video.play();
    }

    ctx.clearRect(0,0,canvas.width,canvas.height); 
    // only draw if loaded and ready
    if(videoContainer !== undefined && videoContainer.ready){ 
        // find the top left of the video on the canvas
        var scale = videoContainer.scale;
        var vidH = videoContainer.video.videoHeight;
        var vidW = videoContainer.video.videoWidth;
        var top = canvas.height / 2 - (vidH /2 ) * scale;
        var left = canvas.width / 2 - (vidW /2 ) * scale;
        // now just draw the video the correct size
        ctx.drawImage(videoContainer.video, 0, 0, 500, 500);
  
        //const imageData = ctx.getImageData(0, 0, 500, 500);
        
        //for(let i=0; i<imageData.data.length; i+=4)
        //{
        //    let gamma = imageData.data[i] + imageData.data[i+1] + imageData.data[i+2];
        //    imageData.data[i+3]=30 + (gamma * 2);
        //}
        //ctx.putImageData(imageData, 0, 0);

        if(videoContainer.video.paused){ // if not playing show the paused screen 
            drawPlayIcon();
        }
    }
    // all done for display 
    // request the next frame in 1/60th of a second
    requestAnimationFrame(updateCanvas);
}

function drawPlayIcon(){
     ctx.fillStyle = "black";  // darken display
     ctx.globalAlpha = 0.5;
     ctx.fillRect(0,0,canvas.width,canvas.height);
     ctx.fillStyle = "#DDD"; // colour of play icon
     ctx.globalAlpha = 0.75; // partly transparent
     ctx.beginPath(); // create the path for the icon
     var size = (canvas.height / 2) * 0.25;  // the size of the icon
     ctx.moveTo(canvas.width/2 + size/2, canvas.height / 2); // start at the pointy end
     ctx.lineTo(canvas.width/2 - size/2, canvas.height / 2 + size);
     ctx.lineTo(canvas.width/2 - size/2, canvas.height / 2 - size);
     ctx.closePath();
     ctx.fill();
     ctx.globalAlpha = 1; // restore alpha
}    

function playPauseClick(){
    if(videoContainer !== undefined && videoContainer.ready){
        if(videoContainer.video.paused){                                 
            videoContainer.video.play();
        }else{
            videoContainer.video.pause();
        }
    }
}

canvas.addEventListener("click",playPauseClick);
