import { starCat as ch0cat} from "./cat-channel-0.js";
import { starCat as ch1cat} from "./cat-channel-1.js";
import { starCat as ch2cat} from "./cat-channel-2.js";
import { starCat as ch3cat} from "./cat-channel-3.js";
import { starCat as ch4cat} from "./cat-channel-4.js";
import { starCat as ch5cat} from "./cat-channel-5.js";
// Channel 6+7 (Podcasts) retreived dynamically later
import { starCat as ch8cat} from "./cat-channel-8.js";

// Update channel selector and show catalogue
let totalChannels = 9;
let toggleChannels = new Array(totalChannels).fill(true, 0);
let selectedChannel = 0;
let showDurations = true;
let showVideoStrip = true;

let querystring = document.location.search.substring(1);
if(querystring != "") {
    for(var c=1; c<=totalChannels; c++) {
        if(querystring.indexOf(c.toString()) == -1) {
            $('#channel' + (c-1) + "row").hide();
            $('#channel' + (c-1) + "toggle").removeClass('bi-' + c + '-square-fill');
            $('#channel' + (c-1) + "toggle").addClass('bi-' + c + '-square');
            toggleChannels[c-1] = false;
        } else {
            $('#channel' + (c-1) + "row").show();
        }
    }
}

let episodeList = new Array(totalChannels);
for(var c=0; c<totalChannels; c++) {
    episodeList[c] = {};
}

episodeList[0].data = ch0cat();
episodeList[1].data = ch1cat();
episodeList[2].data = ch2cat();
episodeList[3].data = ch3cat();
episodeList[4].data = ch4cat();
episodeList[5].data = ch5cat();
episodeList[8].data = ch8cat();

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
setTimeout(autoRefreshCaption, 30000);

function refreshPodcasts()
{
    $.getJSON(`./js/cat-channel-6.json?z=${Math.random()}`, function(json) {
        episodeList[6].data = json;
    });
    $.getJSON(`./js/cat-channel-7.json?z=${Math.random()}`, function(json) {
        episodeList[7].data = json;
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

    // Is this music, if so play at 40% volume
    if(episodeList[selectedChannel].isMusic) {
        // Show/hide PFC overlay
        videoContainer.video.volume = 0.4;
    } else {
        videoContainer.video.volume = 1;
    }

    // Show/hide PFC overlay
    if(episodeList[selectedChannel].isMusic || episodeList[selectedChannel].isPodcast) {
        $('#pfcOverlay').show();
    } else {
        $('#pfcOverlay').hide();
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
                episodeList[channel].endTime = new Date(now.getTime() - (currentPosition * 1000) + (episodeList[channel].data[i].length * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit", hour24: "true" });
                episodeList[channel].perc = Math.floor(100 * (currentPosition / episodeList[channel].data[i].length));
                episodeList[channel].title = episodeList[channel].data[i].title.replace(" - ", ": ").replace("- ", "-").replace("( ", "("); // Leading spaces enforce sort order but don't need to be displayed
                episodeList[channel].description = episodeList[channel].data[i].description.replace("( ", "(").replace("&", "and");
                episodeList[channel].upNext = episodeList[channel].data[nextEpisodeId].title.replace(" - ", ": ").replace("- ", "-");  
                episodeList[channel].source = episodeList[channel].data[i].urlLocal;
                episodeList[channel].isMusic = episodeList[channel].data[i].isMusic;
                episodeList[channel].isPodcast = episodeList[channel].data[i].isPodcast;

                // Overrides for music channel, which shows time for the whole series (album)
                if(episodeList[channel].isMusic) {
                    episodeList[channel].duration = Math.floor(episodeList[channel].data[i].seriesTotalLength / 60); // whole minutes
                    episodeList[channel].endTime = new Date(now.getTime() - ((episodeList[channel].data[i].cumulativeLength - episodeList[channel].data[i].length + currentPosition) * 1000) + (episodeList[channel].data[i].seriesTotalLength * 1000)).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit", hour24: "true" });
                    episodeList[channel].perc = Math.floor(100 * ((episodeList[channel].data[i].cumulativeLength - episodeList[channel].data[i].length + currentPosition) / episodeList[channel].data[i].seriesTotalLength));
                }

                // Optionally remove the series name from upNext if it is the same as what is currently playing
                if(episodeList[channel].title.indexOf(":") >= 0 && episodeList[channel].upNext.indexOf(":") > 0) {
                    var currentShow = episodeList[channel].title.substring(0, episodeList[channel].title.indexOf(":"));
                    var nextShow = episodeList[channel].title.substring(0, episodeList[channel].upNext.indexOf(":"));

                    if(currentShow == nextShow) {   // Remove the name of the show
                        episodeList[channel].upNext = episodeList[channel].upNext.replace(currentShow + ": ", "");
                    }
                }
               
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

            $(`#channel${i}time`).text(`${label}`); 
        }
        else {
            $(`#channel${i}time`).text(`${episodeList[i].endTime}`); 
        }

        updateControlText(`#channel${i}title`, episodeList[i].title);
                
        // If >85% through show up next rather than the description. Radio channels always show description
        var upNext = "(at " + episodeList[i].endTime + ") " + episodeList[i].upNext;
        if(!episodeList[i].isMusic && (episodeList[i].description == "" || episodeList[i].description.indexOf(" ") == -1 || episodeList[i].perc >= 85)) {
            updateControlText(`#channel${i}description`, upNext);
        }
        else {
            updateControlText(`#channel${i}description`, episodeList[i].description.replace("(", "<nobr>(").replace(")", ")</nobr>"));
        }
        
        if(episodeList[i].perc >= 85) {
            $(`#channel${i}progress`).removeClass("bg-warning");
            $(`#channel${i}progress`).addClass("bg-danger");    
        }
        else {
            $(`#channel${i}progress`).removeClass("bg-danger");
            $(`#channel${i}progress`).addClass("bg-warning");
        }
        $(`#channel${i}progress`).css("width", episodeList[i].perc + "%");

        // Update clock
        var clockString = new Date().toLocaleTimeString('eo', { hour12: false }).substring(0, 5);
        if(!showDurations) {
            clockString = "<span class='bi bi-hourglass-bottom'> " + clockString + "</span>";
        }
        else {
            clockString = "<span class='bi bi-hourglass-top'> " + clockString + "</span>";
        }
        $("#currentTime").html(clockString);
    }
}

function updateControlText(control, value) {
    if(document.getElementById(control.substring(1)).innerHTML != value) {
        $(control).fadeOut(function() {
            $(this).html(value)
        }).fadeIn();
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

function toggleGroups() {
    $(".groupOne").fadeToggle(0);
    $(".groupTwo").fadeToggle(0);
    showVideoStrip = !showVideoStrip;
    if(showVideoStrip) {
        $("#videoChannelStrip").css("opacity", "100%");
        $("#audioChannelStrip").css("opacity", "20%");
    } else {
        $("#videoChannelStrip").css("opacity", "20%");
        $("#audioChannelStrip").css("opacity", "100%");
    }
    return false;
}

// Wire up channel selection buttons
for(let i=0; i<totalChannels; i++) {
    $(`#channel${i}`).on('click', function(event) {
        switchChannel(i);
        event.preventDefault();
    }
)};


$("#toggleShowDurations").on('click', function(event) {
    showDurations = !showDurations;
    displayCaption();
    return false;
})

$("#toggleSpeaker").on('click', function(event) {
    if(videoContainer.video.muted) {
        $("#speakerIcon").removeClass("bi-volume-mute");
        $("#speakerIcon").addClass("bi-volume-up-fill");
    }
    else {
        $("#speakerIcon").removeClass("bi-volume-up-fill");
        $("#speakerIcon").addClass("bi-volume-mute");
    }
    videoContainer.video.muted = !videoContainer.video.muted;
  
    return false;
})

// Wire up channel toggle buttons
var $buttons = [0, 1, 2, 3, 4, 5, 6, 7, 8];
$.each($buttons, function(i, v) {
    $('#channelButton' + v).click(function() {
        if((showVideoStrip && v < 6) || (!showVideoStrip && v >= 6)) {
            toggleChannels[v] = !toggleChannels[v];        

            if(toggleChannels[v]) {
                $('#channel' + v.toString() + "row").show();
                $('#channel' + v.toString() + "toggle").removeClass('bi-' + (v+1).toString() + '-square');
                $('#channel' + v.toString() + "toggle").addClass('bi-' + (v+1).toString() + '-square-fill');
            }
            else {
                $('#channel' + v.toString() + "row").hide();
                $('#channel' + v.toString() + "toggle").removeClass('bi-' + (v+1).toString() + '-square-fill');
                $('#channel' + v.toString() + "toggle").addClass('bi-' + (v+1).toString() + '-square');
            }
        }
        else
        {
            // Click on a channel from the other group, so switch over to it
            toggleGroups();
        }
    });
});

// Wire up video canvas event listeners
videoCanvas.addEventListener("click", playPauseClick);
videoCanvas.addEventListener("ended", advanceEpisode);
