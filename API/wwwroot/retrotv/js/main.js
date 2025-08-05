import { starCat as ch1cat} from "./cat-channel-1.js";
import { starCat as ch2cat} from "./cat-channel-2.js";
import { starCat as ch3cat} from "./cat-channel-3.js";
import { starCat as ch4cat} from "./cat-channel-4.js";
import { starCat as ch5cat} from "./cat-channel-5.js";
import { starCat as ch6cat} from "./cat-channel-6.js";

// Update channel selector and show catalogue
let maxChannels = 6;

let selectedChannel = document.location.search.substring(1).slice(-1);
if(!(selectedChannel >= '0' && selectedChannel < maxChannels)) {
    selectedChannel = 0;
}

let episodeList = [{}, {}, {}, {}, {}, {}];
episodeList[0].data = ch1cat();
episodeList[1].data = ch2cat();
episodeList[2].data = ch3cat();
episodeList[3].data = ch4cat();
episodeList[4].data = ch5cat();
episodeList[5].data = ch6cat();

// Set up the canvas
let canvas = document.getElementById("teletextCanvas");
let ctx = canvas.getContext("2d", { willReadFrequently: true });

let video = document.createElement("video"); 
video.autoPlay = false; 
video.loop = false; 
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
    // Update stats for each channel to ensure the banner is accurate
    updateChannelStats();
    console.log(episodeList[selectedChannel].title);

    $('#channel' + selectedChannel).removeClass('active');
    selectedChannel = channel;
    $('#channel' + selectedChannel).addClass('active');

    // Is this audio, if so play at 40% volume
    if(episodeList[selectedChannel].displayParams) {
        videoContainer.video.volume = 1;
    }
    else {
        videoContainer.video.volume = 0.4;
    }

    videoContainer.video.src = episodeList[selectedChannel].source;
    videoContainer.video.currentTime = episodeList[selectedChannel].currentPosition;
    videoContainer.startPosition = episodeList[selectedChannel].currentPosition;
    videoContainer.startPositionTimeStamp = new Date();
    videoContainer.video.play();

    displayCaption();
    $("#planner").fadeToggle();
}

function updateChannelStats(advanceOffset = false) {
    let now = new Date();

    for(let channel=0; channel<maxChannels; channel++) {
        let totalTimes = 0;
        episodeList[channel].data.forEach((e) => totalTimes += e.length);
        let days = Math.floor(totalTimes / 86400, 0);
        let hours = Math.round((totalTimes - (days * 86400)) / 3600, 1)
        console.log(`Channel ${channel+1} total length, ${days} days, ${hours} hours`);
        
        // Select the current episode and time based on seconds since epoch
        let dayPosition = Math.floor(((now.getTime() / 1000) + (advanceOffset ? 1 : 0)) % totalTimes);

        // From this position, find out which episode this falls on
        var sum = 0;
        for (let i=0; i<episodeList[channel].data.length; i++) {
            if(sum + episodeList[channel].data[i].length > dayPosition) {    
                let nextEpisodeId = (i == episodeList[channel].data.length - 1) ? 0 : i + 1;
                
                let currentPosition = dayPosition - sum;
                episodeList[channel].currentPosition = currentPosition;
                episodeList[channel].startTime = new Date(now.getTime() - (currentPosition * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
                episodeList[channel].endTime = new Date(now.getTime() - (currentPosition * 1000) + (episodeList[channel].data[i].length * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
                episodeList[channel].episodeId = i;
                episodeList[channel].perc = Math.floor(100 * (currentPosition / episodeList[channel].data[i].length));
                episodeList[channel].title = episodeList[channel].data[i].title;
                episodeList[channel].description = episodeList[channel].data[i].description;
                episodeList[channel].upNext = episodeList[channel].data[nextEpisodeId].shortName;                
                episodeList[channel].displayParams = episodeList[channel].data[i].displayParams;                
                episodeList[channel].source = episodeList[channel].data[i].urlLocal;
                break;
            }
            else {
                sum += episodeList[channel].data[i].length;
            }
        }
    }
}

function advanceEpisode() {
    // Move to the start of the next episode
    updateChannelStats(true);
   
    videoContainer.video.src = episodeList[selectedChannel].source;
    videoContainer.video.currentTime = 0;
    videoContainer.startPosition = 0;
    videoContainer.startPositionTimeStamp = new Date();
    videoContainer.video.play();

    displayCaption();
}

function displayCaption() {
    for(let i=0; i<maxChannels; i++) {
        $(`#channel${i}start`).text(`${episodeList[i].startTime} - ${episodeList[i].endTime}`); 
        $(`#channel${i}title`).text(episodeList[i].title);
        $(`#channel${i}description`).text(episodeList[i].description);
        $(`#channel${i}upNext`).text("Up next: " + episodeList[i].upNext);
        $(`#channel${i}progress`).css("width", episodeList[i].perc + "%");
    }
}

function readyToPlayVideo(event) { 
    videoContainer.ready = true;
    console.log('Episode ready to play, total length ' + video.duration);
    console.log('Episode title', episodeList[selectedChannel].title);

    // The video can be played so hand it off to the display function
    requestAnimationFrame(updateCanvas);
}

function updateCanvas() {
    if(videoContainer.ready) { 
        if(video.ended) {
            // Advance to next episode and update container details
            advanceEpisode();
        }
       
        let episodeId = episodeList[selectedChannel].episodeId;
        let dp = episodeList[selectedChannel].data[episodeId].displayParams;
        if(dp) {
            canvas.width = dp[0];
            canvas.height = dp[1];
            ctx.clearRect(0,0,canvas.width,canvas.height); 
            ctx.drawImage(videoContainer.video, 0, 0);
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
            $("#planner").fadeToggle();
        }
    }
}

canvas.addEventListener("click", playPauseClick);

// Wire up channel selection buttons
for(let i=0; i<maxChannels; i++) {
    $(`#channel${i}`).on('click', function(event) {
        switchChannel(i);
        event.preventDefault();
    }
)};

