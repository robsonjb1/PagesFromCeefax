import { starCat as ch0cat} from "./cat-channel-0.js";
import { starCat as ch1cat} from "./cat-channel-1.js";
import { starCat as ch2cat} from "./cat-channel-2.js";
import { starCat as ch3cat} from "./cat-channel-3.js";
import { starCat as ch4cat} from "./cat-channel-4.js";
// Channel 5+6 (Podcasts) retreived dynamically later
import { starCat as ch7cat} from "./cat-channel-7.js";

// Update channel selector and show catalogue
let totalChannels = 8;
let selectedChannel = 0;
let showDurations = true;

let querystring = document.location.search.substring(1);
if(querystring != "") {
    for(var c=1; c<=totalChannels; c++) {
        if(querystring.indexOf(c.toString()) == -1) {
            document.getElementById("channel" + (c-1).toString() + "row").style = "display:none";
        }
    }
}

let episodeList = [{}, {}, {}, {}, {}, {}, {}, {}];
episodeList[0].data = ch0cat();
episodeList[1].data = ch1cat();
episodeList[2].data = ch2cat();
episodeList[3].data = ch3cat();
episodeList[4].data = ch4cat();
episodeList[7].data = ch7cat();

$.ajaxSetup({
    async: false
});

// Check if the podcast feed has changed
refreshPodcasts();

// Set up the canvas
let video = document.getElementById("videoCanvas");
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
autoRefreshCaption();

function refreshPodcasts()
{
    $.getJSON(`./js/cat-channel-5.json?z=${Math.random()}`, function(json) {
        episodeList[5].data = json;
    });
    $.getJSON(`./js/cat-channel-6.json?z=${Math.random()}`, function(json) {
        episodeList[6].data = json;
    });
}

function autoRefreshCaption()
{
    // Check if the podcast feed has changed (async)
    refreshPodcasts();

    updateChannelStats();
    displayCaption();
    setTimeout(autoRefreshCaption, 30000); // If the banner is showing, update the content every 30 seconds
}

function switchChannel(channel)
{
    // Update stats for each channel to ensure the banner is accurate
    updateChannelStats();
   
    $('#channel' + selectedChannel).removeClass('active');
    selectedChannel = channel;
    $('#channel' + selectedChannel).addClass('active');

    // Is this audio, if so play at 40% volume
    if(episodeList[selectedChannel].displayParams) {
        videoContainer.video.volume = 1;
         // Hide the PFC frame
         $('#pfcOverlay').hide();
    }
    else {
        videoContainer.video.volume = 0.4;
        // Show the PFC frame
        $('#pfcOverlay').show();
    }

    videoContainer.video.pause();
    videoContainer.video.src = episodeList[selectedChannel].source;
    videoContainer.video.currentTime = episodeList[selectedChannel].currentPosition;
    videoContainer.startPosition = episodeList[selectedChannel].currentPosition;
    videoContainer.startPositionTimeStamp = new Date();

    videoContainer.video.load();
    videoContainer.video.play();

    displayCaption();
}

function updateChannelStats(advanceOffset = false) {
    let now = new Date();

    for(let channel=0; channel<totalChannels; channel++) {
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

                episodeList[channel].duration = Math.floor(episodeList[channel].data[i].length / 60);   // whole minutes 
                episodeList[channel].currentPosition = currentPosition;
                episodeList[channel].startTime = new Date(now.getTime() - (currentPosition * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit", hour24: "true" });
                episodeList[channel].endTime = new Date(now.getTime() - (currentPosition * 1000) + (episodeList[channel].data[i].length * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit", hour24: "true" });
                episodeList[channel].episodeId = i;
                episodeList[channel].perc = Math.floor(100 * (currentPosition / episodeList[channel].data[i].length));
                episodeList[channel].title = episodeList[channel].data[i].title.replace(" - ", ": ").replace("- ", "-").replace("( ", "("); // Leading spaces enforce sort order but don't need to be displayed
                episodeList[channel].description = episodeList[channel].data[i].description.replace("( ", "(");
                episodeList[channel].upNext = episodeList[channel].data[nextEpisodeId].title.replace(" - ", ": ").replace("- ", "-");  
              
                // Optionally remove the series name from upNext if it is the same as what is currently playing
                if(episodeList[channel].title.indexOf(":") >= 0 && episodeList[channel].upNext.indexOf(":") > 0) {
                    var currentShow = episodeList[channel].title.substring(0, episodeList[channel].title.indexOf(":"));
                    var nextShow = episodeList[channel].title.substring(0, episodeList[channel].upNext.indexOf(":"));

                    if(currentShow == nextShow) {   // Remove the name of the show
                        episodeList[channel].upNext = episodeList[channel].upNext.replace(currentShow + ": ", "");
                    }
                    //else {
                    //    episodeList[channel].title += "&nbsp; &#x263e;";
                    //}
                }

                episodeList[channel].displayParams = episodeList[channel].data[i].displayParams;                
                episodeList[channel].source = episodeList[channel].data[i].urlLocal;
                episodeList[channel].suppressUpNext = episodeList[channel].data[i].suppressUpNext;
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
   
    videoContainer.video.pause();
    videoContainer.video.src = episodeList[selectedChannel].source;
    videoContainer.video.currentTime = 0;
    videoContainer.startPosition = 0;
    videoContainer.startPositionTimeStamp = new Date();
    videoContainer.video.load();
    videoContainer.video.play();

    displayCaption();
}

function displayCaption() {
    for(let i=0; i<totalChannels; i++) {
        if(showDurations) {
            var label = `${episodeList[i].duration} mins`;

            if(episodeList[i].duration > 60) {
                var hours = Math.floor(episodeList[i].duration / 60);
                var mins = episodeList[i].duration - (hours * 60);
                label = `${hours}hr ${mins}m`;
            }

            $(`#channel${i}start`).text(`${label}`); 
        }
        else {
            $(`#channel${i}start`).text(`${episodeList[i].endTime}`); 
        }

        $(`#channel${i}title`).html(episodeList[i].title);
                
        // If >85% through show up next rather than the description. Radio channels always show description
        var upNext = "(at " + episodeList[i].endTime + ") " + episodeList[i].upNext;
        if(!episodeList[i].suppressUpNext && (episodeList[i].description == "" || episodeList[i].description.indexOf(" ") == -1 || episodeList[i].perc >= 85)) {
            $(`#channel${i}description`).html(upNext);
        }
        else {
            $(`#channel${i}description`).html(episodeList[i].description);
        }
        
        if(episodeList[i].perc >= 85) {
            $(`#channel${i}progress`).removeClass("bg-warning");
            $(`#channel${i}progress`).addClass("bg-danger");    
        }
        else {
            $(`#channel${i}progress`).removeClass("bg-danger");
            $(`#channel${i}progress`).addClass("bg-warning");
        }

        $(`#channel${i}upNext`).text(upNext);
        $(`#channel${i}progress`).css("width", episodeList[i].perc + "%");

        // Update clock
        $("#currentTime").text((new Date()).toLocaleTimeString('eo', { hour12: false }).substring(0, 5));
    }
}

function readyToPlayVideo(event) { 
    videoContainer.ready = true;
    console.log('Episode ready to play, total length ' + video.duration);
    console.log('Episode title', episodeList[selectedChannel].title);
}

function playPauseClick() {
    if(videoContainer.ready){
        if(videoContainer.video.paused) {  
            // When playing after a pause, advance to where the stream should be now    
            video.currentTime = videoContainer.startPosition + ((new Date() - videoContainer.startPositionTimeStamp) / 1000);
            videoContainer.video.play();
            $("#planner").fadeOut();
        }
        else {
            // Update stats for each channel
            updateChannelStats();
            displayCaption();
            $("#planner").fadeToggle();
        }
    }
}

// Wire up channel selection buttons
for(let i=0; i<totalChannels; i++) {
    $(`#channel${i}`).on('click', function(event) {
        switchChannel(i);
        event.preventDefault();
    }
)};

$("#toggleGroups").on('click', function(event) {
    $(".groupOne").fadeToggle(0);
    $(".groupTwo").fadeToggle(0);
    return false;
})

$("#toggleShowDurations").on('click', function(event) {
    showDurations = !showDurations;
    displayCaption();
    return false;
})

$("#toggleSpeaker").on('click', function(event) {
    if(videoContainer.video.muted) {
        $("#speakerIcon").removeClass("bi-volume-mute");
        $("#speakerIcon").addClass("bi-volume-up");
    }
    else {
        $("#speakerIcon").removeClass("bi-volume-up");
        $("#speakerIcon").addClass("bi-volume-mute");
    }
    videoContainer.video.muted = !videoContainer.video.muted;
  
    return false;
})

// Wire up video canvas event listeners
videoCanvas.addEventListener("click", playPauseClick);
videoCanvas.addEventListener("ended", advanceEpisode);
