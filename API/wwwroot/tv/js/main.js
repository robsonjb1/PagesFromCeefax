import { starCat } from "./cat.js";

let episodeList = starCat();

let canvas = document.getElementById("teletextCanvas"); // get the canvas from the page
canvas.width = 500; 
canvas.height = 500;

let ctx = canvas.getContext("2d", { willReadFrequently: true });

let videoContainer; //  to hold video and associated info
let video = document.createElement("video"); // create a video element

// the video will now begin to load.
// As some additional info is needed we will place the video in a
// containing object for convenience
video.autoPlay = false; // ensure that the video does not auto play
video.loop = true; // set the video to loop.
videoContainer = {  // we will add properties as needed
     video : video,
     ready : false,   
     startPosition : null,
     startPositionTimeStamp : null,
};

video.addEventListener('loadedmetadata',function() {
      readyToPlayVideo();
});

function readyToPlayVideo(event){ // this is a referance to the video
    // the video may not match the canvas size so find a scale to fit

    videoContainer.ready = true;
    console.log('Episode ready to play, total length ' + video.duration);

    // the video can be played so hand it off to the display function
    requestAnimationFrame(updateCanvas);
}

selectEpisodePosition();

function selectEpisodePosition()
{
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
            video.src = episodeList[currentEpisode].url;
    
            videoContainer.startPosition = dayPosition - sum;
            videoContainer.startPositionTimeStamp = now;
            
            console.log('Moving to episode ' + currentEpisode + " position " + videoContainer.startPosition);
            console.log('This episode is', episodeList[i].title);
    
            const episodeStartTime = new Date(now - (videoContainer.startPosition * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
            const nextEpisodeStarts = new Date(now - (videoContainer.startPosition * 1000) + (episodeList[currentEpisode].length * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
            
            console.log('Episode started at', episodeStartTime);
            let nextEpisodeTemp = currentEpisode + 1;
            if(nextEpisodeTemp === episodeList.length) {
                nextEpisodeTemp = 0;
            }
            console.log('Next episode is', episodeList[nextEpisodeTemp].title);
            console.log('Next episode starts at', nextEpisodeStarts);
    
            $("#nowTime").text(episodeStartTime);
            $("#nextTime").text(nextEpisodeStarts);
            $("#nowTitle").text(episodeList[i].title);
            $("#nextTitle").text(episodeList[nextEpisodeTemp].title);
            
            $("#nowNext").delay(3000).fadeIn();
            $("#nowNext").delay(5000).fadeOut();
            
            break;
        }
        else
        {
            sum += episodeList[i].length;
        }
    }
}

function updateCanvas()
{
    if(video.currentTime >= video.duration - 1000) {
        // Advance to next episode after one second to ensure the episode tracker picks the right episode
        setTimeout(selectEpisodePosition, 1500);
        video.play();
    }

    ctx.clearRect(0,0,canvas.width,canvas.height); 

    // only draw if loaded and ready
    if(videoContainer !== undefined && videoContainer.ready){ 
        
        ctx.drawImage(videoContainer.video, 0, 0, 500, 500);

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
     ctx.moveTo(canvas.width/2 + size, canvas.height / 1.75 - size); // start at the pointy end
     ctx.lineTo(canvas.width/2 - size, canvas.height / 1.75 + size / 2);
     ctx.lineTo(canvas.width/2 - size/2.5, canvas.height / 1.75 - size /2);
     ctx.lineTo(canvas.width/2 - size*1.25, canvas.height / 1.75 - size);
     ctx.closePath();
     ctx.fill();
     ctx.globalAlpha = 1; // restore alpha
}    

function playPauseClick(){
    if(videoContainer !== undefined && videoContainer.ready){
        if(videoContainer.video.paused){      
            video.currentTime = videoContainer.startPosition + ((new Date() - videoContainer.startPositionTimeStamp) / 1000);
                           
            videoContainer.video.play();
            $("#nowNext").fadeOut();
        }else{
            videoContainer.video.pause();
            $("#nowNext").fadeIn();
        }
    }
}

canvas.addEventListener("click",playPauseClick);
