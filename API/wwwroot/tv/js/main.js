import { starCat as blakes7cat} from "./blakes7-cat.js";
import { starCat as drwhocat} from "./drwho-cat.js";
import { starCat as comedycat} from "./comedy-cat.js";
import { starCat as totpcat} from "./totp-cat.js";

// Update channel selector and show catalogue
let selectedChannel = document.location.search.substring(1).slice(-1);
if(!(selectedChannel >= '0' && selectedChannel <= 3))
{
    selectedChannel = 0;
}

let episodeList = [{}, {}, {}, {}];
episodeList[0].data = blakes7cat();
episodeList[1].data = comedycat();
episodeList[2].data = drwhocat();
episodeList[3].data = totpcat();

// Set up the canvas
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
     ready : false
};

// Start video
switchChannel(selectedChannel);

// Functions
function switchChannel(channel)
{
    $('#channel' + selectedChannel).removeClass('active');
    selectedChannel = channel;
    $('#channel' + selectedChannel).addClass('active');

    // Update stats for each channel
    updateChannelStats();
   
    videoContainer.video.src = episodeList[selectedChannel].source;
    videoContainer.video.currentTime = episodeList[selectedChannel].currentPosition;
    
    let now = new Date();
    videoContainer.startPosition = episodeList[selectedChannel].currentPosition;
    videoContainer.startPositionTimeStamp = now;

    displayCaption();
}

function updateChannelStats() {
    let now = new Date();

    for(let channel=0; channel<4; channel++) {
        let totalTimes = 0;

        episodeList[channel].data.forEach((e) => totalTimes += e.length);
        let days = Math.floor(totalTimes / 86400, 0);
        let hours = Math.round((totalTimes - (days * 86400)) / 3600, 1)
        console.log(`Channel ${channel} total length, ${days} days, ${hours} hours`);
        
        // Select the current episode and time based on how far into the current month we are
        let dayPosition = Math.floor(((now.getDate() * 86400) + (now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) % totalTimes);
    
        // From this position, find out which episode this falls on
        var sum = 0;
        for (let i=0; i<episodeList[channel].data.length; i++) {
            if(sum + episodeList[channel].data[i].length > dayPosition) {    
                let currentPosition = dayPosition - sum;
                episodeList[channel].currentPosition = currentPosition;
                episodeList[channel].startTime = new Date(now.getTime() - (currentPosition * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
                episodeList[channel].endTime = new Date(now.getTime() - (currentPosition * 1000) + (episodeList[channel].data[i].length * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
                episodeList[channel].episodeId = i;
                episodeList[channel].title = episodeList[channel].data[i].title;
                episodeList[channel].source = getOneDriveLink(channel, i);
                break;
            }
            else {
                sum += episodeList[channel].data[i].length;
            }
        }
    }
}

function getOneDriveLink(channel, episodeId) {
    if(episodeList[channel].data[episodeId].urlProcessed) {
        return episodeList[channel].data[episodeId].urlProcessed;
    } else
    {
        let rawUrl = episodeList[channel].data[episodeId].url;
        let encodedUrl = btoa(rawUrl).replace("=", "").replace("/", "_").replace("+", "-");
        return "https://api.onedrive.com/v1.0/shares/u!" + encodedUrl + "/root/content";
    }
}

function advanceEpisode() {
    // Move to the start of the next episode
    let episodeId = episodeList[selectedChannel].episodeId;

    episodeId++;
    if(episodeId === episodeList[selectedChannel].data.length)
    {
        episodeId = 0;
    }
    
    let now = new Date();
    episodeList[selectedChannel].episodeId = episodeId;
    episodeList[selectedChannel].currentPosition = 0;
    episodeList[selectedChannel].startTime = new Date(now.getTime()).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    episodeList[selectedChannel].endTime = new Date(now.getTime() + (episodeList[selectedChannel].data[episodeId].length * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    episodeList[selectedChannel].title = episodeList[selectedChannel].data[episodeId].title;
    episodeList[selectedChannel].source = getOneDriveLink(selectedChannel, episodeId);
    
    videoContainer.video.src = episodeList[selectedChannel].source;
    videoContainer.video.currentTime = 0;
    videoContainer.startPosition = 0;
    videoContainer.startPositionTimeStamp = now;

    videoContainer.video.play();
}

function displayCaption() {
    for(let i=0; i<4; i++) {
        $(`#channel${i}time`).text(`${episodeList[i].startTime} - ${episodeList[i].endTime}`);
        $(`#channel${i}title`).text(episodeList[i].title);
    }

    $("#planner").fadeToggle();
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
       
        let episodeId = episodeList[selectedChannel].episodeId;
        let dp = episodeList[selectedChannel].data[episodeId].displayParams;
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
            $("#planner").fadeOut();
        }else{
            // Update stats for each channel
            updateChannelStats();
            displayCaption();
        }
    }
}

canvas.addEventListener("click",playPauseClick);

// Wire up channel selection buttons
for(let i=0; i<4; i++) {
    $(`#channel${i}`).on('click', function(event) {
        switchChannel(i);
        videoContainer.video.play();
        event.preventDefault();
    }
)};

