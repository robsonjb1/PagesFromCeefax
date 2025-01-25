import { starCat } from "./cat.js";

let episodeList = starCat();

let canvas = document.getElementById("teletextCanvas");
canvas.width = 500; 
canvas.height = 500;

let ctx = canvas.getContext("2d", { willReadFrequently: true });

let video = document.createElement("video"); 
video.autoPlay = false; 
video.loop = true; 
video.addEventListener('loadedmetadata',function() {
    readyToPlayVideo();
});

let videoContainer = {
     video : video,
     ready : false,   
     startPosition : null,
     startPositionTimeStamp : null,
     currentEpisode : 0
};

let captionContainer = {
    nowTime : null,
    nextTime : null,
    nowTitle : null,
    nextTitle : null
}

initialiseVideo();

function initialiseVideo() {
    let now = new Date();
    let totalTimes = 0;
    episodeList.forEach((e) => totalTimes += e.length);

    // Time into day
    let dayPosition = Math.floor(((now.getDate() * 86400) + (now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) % totalTimes);
   
    // Find out which episode this is
    var sum = 0;

    for (let i=0; i<episodeList.length; i++) {
        if(sum+episodeList[i].length > dayPosition) {
            videoContainer.currentEpisode = i;
            videoContainer.video.src = episodeList[videoContainer.currentEpisode].url;
            
            videoContainer.startPosition = dayPosition - sum;
            videoContainer.startPositionTimeStamp = now;
        
            captionContainer.nowTime = new Date(now.getTime() - (videoContainer.startPosition * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
            captionContainer.nextTime = new Date(now.getTime() - (videoContainer.startPosition * 1000) + (episodeList[videoContainer.currentEpisode].length * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
            
            let nextEpisode = videoContainer.currentEpisode + 1;
            if(nextEpisode === episodeList.length) {
                nextEpisode = 0;
            }
        
            captionContainer.nowTitle = episodeList[videoContainer.currentEpisode].title;
            captionContainer.nextTitle = episodeList[nextEpisode].title;

            displayCaption();
            break;
        }
        else
        {
            sum += episodeList[i].length;
        }
    }
}

function advanceEpisode() {
    videoContainer.currentEpisode++;
    if(videoContainer.currentEpisode == episodeList.length) {
        videoContainer.currentEpisode = 0;
    }
    videoContainer.video.src = episodeList[videoContainer.currentEpisode].url;
    videoContainer.currentTime = 0 ;
    videoContainer.startPosition = 0;
    videoContainer.startPositionTimeStamp = new Date();

    let nextEpisode = videoContainer.currentEpisode + 1;
    if(nextEpisode === episodeList.length) {
        nextEpisode = 0;
    }
    video.play();

    let now = new Date()
    captionContainer.nowTime = now.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    captionContainer.nextTime = new Date(now.getTime() + episodeList[videoContainer.currentEpisode].length * 1000).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    captionContainer.nowTitle = episodeList[videoContainer.currentEpisode].title;
    captionContainer.nextTitle = episodeList[nextEpisode].title;

    displayCaption();
}

function displayCaption() {
    $("#nowTime").text(captionContainer.nowTime);
    $("#nextTime").text(captionContainer.nextTime);
    $("#nowTitle").text(captionContainer.nowTitle);
    $("#nextTitle").text(captionContainer.nextTitle);
    
    $("#nowNext").fadeIn();
    $("#nowNext").delay(5000).fadeOut();
}

function readyToPlayVideo(event) { 
    videoContainer.ready = true;
    console.log('Episode ready to play, total length ' + video.duration);

    // the video can be played so hand it off to the display function
    requestAnimationFrame(updateCanvas);
}

function updateCanvas() {
    // only draw if loaded and ready
    if(videoContainer.ready) { 
        if(video.currentTime >= video.duration - 1) {
            // Advance to next episode and update container details
            advanceEpisode();
        }

        ctx.clearRect(0,0,canvas.width,canvas.height); 
        ctx.drawImage(videoContainer.video, 0, 0, 500, 500);
        
        if(videoContainer.video.paused) { 
            drawPlayIcon();
        }
    }

    // all done for display 
    // request the next frame in 1/60th of a second
    requestAnimationFrame(updateCanvas);
}

function drawPlayIcon() {
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

function playPauseClick() {
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
