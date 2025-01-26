import { starCat } from "./cat.js";

// Initial setup
let episodeList = starCat();

let canvas = document.getElementById("teletextCanvas");
canvas.width = 600; 
canvas.height = 600;

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

// Start video
initialiseVideo();

// Functions
function initialiseVideo() {
    let now = new Date();
    let totalTimes = 0;
    episodeList.forEach((e) => totalTimes += e.length);
    console.log('Stream total length ', totalTimes)

    // How far into the episode stream are we based on how far into the current month we are
    let dayPosition = Math.floor(((now.getDate() * 86400) + (now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) % totalTimes);
   
    // From this position, find out which episode this falls on
    var sum = 0;
    for (let i=0; i<episodeList.length; i++) {
        if(sum + episodeList[i].length > dayPosition) {
            videoContainer.currentEpisode = i;

            // Uncomment when adding new episodes
            //videoContainer.currentEpisode = 17;
            //console.log(episodeList[videoContainer.currentEpisode].title);

            videoContainer.video.src = getOneDriveLink(episodeList[videoContainer.currentEpisode].url);
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
        else {
            sum += episodeList[i].length;
        }
    }
}

function getOneDriveLink(rawUrl) {
    let encodedUrl = btoa(rawUrl).replace("=", "").replace("/", "_").replace("+", "-");
    return "https://api.onedrive.com/v1.0/shares/u!" + encodedUrl + "/root/content";
}

function advanceEpisode() {
    // Move to the start of the next episode
    videoContainer.currentEpisode++;
    if(videoContainer.currentEpisode === episodeList.length) {
        videoContainer.currentEpisode = 0;
    }
    videoContainer.video.src = getOneDriveLink(episodeList[videoContainer.currentEpisode].url);
    videoContainer.currentTime = 0;
    videoContainer.startPosition = 0;
    videoContainer.startPositionTimeStamp = new Date();

    let nextEpisode = videoContainer.currentEpisode + 1;
    if(nextEpisode === episodeList.length) {
        nextEpisode = 0;
    }
    video.play();

    // Update caption details
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

    // The video can be played so hand it off to the display function
    requestAnimationFrame(updateCanvas);
}

function updateCanvas() {
    if(videoContainer.ready) { 
        if(video.currentTime >= video.duration - 1) {
            // Advance to next episode and update container details
            advanceEpisode();
        }

        ctx.clearRect(0,0,canvas.width,canvas.height); 
        //ctx.drawImage(videoContainer.video, 120,0,700,500,0,0,500,500);
        //, 160,0,900,1100,0,0,600,900);

        let dp = episodeList[videoContainer.currentEpisode].displayParams;
        if(dp) {
            ctx.drawImage(videoContainer.video, dp[0], dp[1], dp[2], dp[3], dp[4], dp[5], dp[6], dp[7]);
        }
        else {
            ctx.drawImage(videoContainer.video, 0, 0, 600, 600);
        }
        
        if(videoContainer.video.paused) { 
            drawPlayIcon();
        }
    }

    // Request the next frame
    requestAnimationFrame(updateCanvas);
}

function drawPlayIcon() {
    // Attempt to draw the federation logo
    ctx.fillStyle = "black";
    ctx.globalAlpha = 0.5;
    ctx.fillRect(0,0,canvas.width,canvas.height);
    ctx.fillStyle = "#DDD"; // Colour of play icon
    ctx.globalAlpha = 0.75; // Transparency
    ctx.beginPath(); 
    var size = (canvas.height / 2) * 0.25; 
    ctx.moveTo(canvas.width/2 + size, canvas.height / 1.75 - size); // Start of the rightmost point
    ctx.lineTo(canvas.width/2 - size, canvas.height / 1.75 + size / 2);
    ctx.lineTo(canvas.width/2 - size/2.5, canvas.height / 1.75 - size /2);
    ctx.lineTo(canvas.width/2 - size*1.25, canvas.height / 1.75 - size);
    ctx.closePath();
    ctx.fill();
    ctx.globalAlpha = 1; // Restore alpha
}    

function playPauseClick() {
    if(videoContainer.ready){
        if(videoContainer.video.paused){  
            // When playing after a pause, advance to where the stream should be now    
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
