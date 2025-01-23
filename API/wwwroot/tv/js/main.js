import { starCat } from "./cat.js";

let episodeList = starCat();

let canvas = document.getElementById("teletextCanvas"); // get the canvas from the page
canvas.width = 500; 
canvas.height = 500;

let ctx = canvas.getContext("2d", { willReadFrequently: true });

let videoContainer; // object to hold video and associated info
let video = document.createElement("video"); // create a video element

// the video will now begin to load.
// As some additional info is needed we will place the video in a
// containing object for convenience
video.autoPlay = false; // ensure that the video does not auto play
video.loop = true; // set the video to loop.
videoContainer = {  // we will add properties as needed
     video : video,
     ready : false,   
};

let now = new Date();
let totalTimes = 0;
episodeList.forEach((e) => totalTimes += e.length);

// Time into day
let dayPosition = Math.floor(((now.getDate() * 86400) + (now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) % totalTimes);
let currentEpisode = 0;

// Find out which episode this is
var sum = 0;

for (let i=0; i<episodeList.length; i++)
{
    if(sum+episodeList[i].length > dayPosition)
    {
        currentEpisode = i;
        video.currentTime = dayPosition - sum;
        
        video.src = episodeList[currentEpisode].url;
        console.log('Moving to episode ' + currentEpisode + " position " + video.currentTime);
        console.log('This episode is', episodeList[i].title);

        const episodeStartTime = new Date(now - (video.currentTime * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
        const nextEpisodeStarts = new Date(now - (video.currentTime * 1000) + (episodeList[currentEpisode].length * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
        
        console.log('Next episode starts at', nextEpisodeStarts);
        console.log('Episode started at', episodeStartTime);
        let nextEpisodeTemp = currentEpisode + 1;
        if(nextEpisodeTemp === episodeList.length) {
            nextEpisodeTemp = 0;
        }
        console.log('Next episode is', episodeList[nextEpisodeTemp].title);
        



        break;
    }
    else
    {
        sum += episodeList[i].length;
    }
}

// https://learn.microsoft.com/en-us/graph/api/shares-get?view=graph-rest-1.0&tabs=http#encoding-sharing-urls


video.oncanplay = readyToPlayVideo; // set the event to the play function that 
                                  // can be found below
function readyToPlayVideo(event){ // this is a referance to the video
    // the video may not match the canvas size so find a scale to fit
    
    videoContainer.ready = true;
    console.log('Episode ready to play, total length ' + video.duration);

    // the video can be played so hand it off to the display function
    requestAnimationFrame(updateCanvas);
}

function updateCanvas()
{
    if(video.currentTime >= video.duration - 1) {
        // Advance to next episode
        currentEpisode++;
        if(currentEpisode == episodeList.length) {
            currentEpisode = 0;
        }
        console.log('Moving to episode ' + currentEpisode + " position 0");
        video.src = episodeList[currentEpisode].url;
        video.currentTime = 0;
        video.play();
    }

    ctx.clearRect(0,0,canvas.width,canvas.height); 
    // only draw if loaded and ready
    if(videoContainer !== undefined && videoContainer.ready){ 
        // find the top left of the video on the canvas
        
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
