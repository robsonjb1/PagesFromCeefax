// Initialisation
let totalChannels = 11;
let selectedChannel = 10;
let showIdentNext = true;
let settingsVisible = false;
let visibleChannelGroup = 1;
var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

// Initialise episode list
let episodeList = new Array(totalChannels+1);
for(var c=0; c<=totalChannels; c++) {
    episodeList[c] = {};
}

$.ajaxSetup({
    async: false
});

// Load the channel feeds
refreshChannelFeeds();

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
setTimeout(autoRefreshCaption, 10000);          // 10 seconds
setTimeout(autoRefreshChannelFeeds, 600000);    // 10 minutes
switchChannel(selectedChannel);

// Initialise the group lists
$("#channelGrouping2").addClass("initially-hidden");

function refreshChannelFeeds() {
    for(var i=1; i<=totalChannels; i++) {
        $.getJSON(`./js/cat-channel-${i}.json?z=${Math.random()}`, function(json) {
            episodeList[i].data = json.episodeList;
            episodeList[i].lastRefresh = json.lastRefresh;
        });
    }
}

function autoRefreshCaption() {
    setTimeout(autoRefreshCaption, 10000); // If the banner is showing, update the content every 10 seconds
    updateChannelStats();
    displayCaption();
}

function autoRefreshChannelFeeds() {
    setTimeout(autoRefreshChannelFeeds, 600000); // Update the channel feeds every 10 minutes
    refreshChannelFeeds();
}


function switchChannel(channel) {
    showIdentNext = true;

    // Update stats for each channel to ensure the banner is accurate
    updateChannelStats();
   
    $(`#channel${selectedChannel}button`).removeClass('active');
    selectedChannel = channel;
    $(`#channel${selectedChannel}button`).addClass('active');

    // Show/hide PFC overlay
    if(episodeList[selectedChannel].isMusic || episodeList[selectedChannel].isPodcast) {
        $('#pfcOverlay').show();
    } else {
        $('#pfcOverlay').hide();
    }

    if(selectedChannel >= 0) {    
        videoContainer.video.pause();
        videoContainer.video.src = episodeList[selectedChannel].source;
        videoContainer.video.currentTime = episodeList[selectedChannel].currentPosition;
        videoContainer.startPosition = episodeList[selectedChannel].currentPosition;
        videoContainer.startPositionTimeStamp = new Date();

        videoContainer.video.load();
        videoContainer.video.play();

        displayCaption();
    }
}

function updateChannelStats(advanceOffset = false) {
    let now = new Date();

    for(let channel=1; channel<=totalChannels; channel++) {
        let totalTimes = 0;
        episodeList[channel].data.forEach((e) => totalTimes += e.length);
        let days = Math.floor(totalTimes / 86400, 0);
        let hours = Math.round((totalTimes - (days * 86400)) / 3600, 1)
  
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
                episodeList[channel].perc = Math.floor(100 * (currentPosition / episodeList[channel].data[i].length));
                episodeList[channel].title = episodeList[channel].data[i].title;
                episodeList[channel].description = episodeList[channel].data[i].description.replace("( ", "(").replace("&", "and");
                episodeList[channel].upNext = episodeList[channel].data[nextEpisodeId].title;
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
                if(episodeList[channel].title.indexOf(":") >= 0 && episodeList[channel].upNext.indexOf(":") >= 0) {
                    var currentShow = episodeList[channel].title.substring(0, episodeList[channel].title.indexOf(":"));
                    var nextShow = episodeList[channel].upNext.substring(0, episodeList[channel].upNext.indexOf(":"));

                    if(currentShow == nextShow) {   // Remove the name of the show
                        episodeList[channel].upNext = episodeList[channel].data[nextEpisodeId].shortName;
                    }
                }

                // Display the currently playing show on the settings table
                if(days != 0) {
                    $(`#settings${channel}description`).text(`${episodeList[channel].lastRefresh} (${episodeList[channel].data.length} items, ${days} days, ${hours} hours)`); 
                }
                else {
                    $(`#settings${channel}description`).text(`${episodeList[channel].lastRefresh} (${episodeList[channel].data.length} items, ${hours} hours)`); 
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
    if(!episodeList[selectedChannel].isMusic && !episodeList[selectedChannel].isPodcast && showIdentNext) {
        // Display the ident
        startNewVideo("media/RetroTVIdent.mp4");
        
        // Next time round, show the next episode
        showIdentNext = false;
    }
    else {
        // Move to the start of the next episode
        updateChannelStats(true);
        startNewVideo(episodeList[selectedChannel].source);
        displayCaption();

        // Next time round, show the channel ident banner
        showIdentNext = true;
    }
}

function startNewVideo(source) {
    videoContainer.video.pause();
    videoContainer.video.src = source;
    videoContainer.video.currentTime = 0;
    videoContainer.startPosition = 0;
    videoContainer.startPositionTimeStamp = new Date();
    videoContainer.video.load();
    videoContainer.video.play();
}

function displayCaption() {
    for(let i=1; i<=totalChannels; i++) {
        var label = `${episodeList[i].duration} mins`;

        if(episodeList[i].duration > 60) {
            var hours = Math.floor(episodeList[i].duration / 60);
            var mins = episodeList[i].duration - (hours * 60);
            label = `${hours}hr ${mins}m`;
        }
        $(`#channel${i}time`).text(label);
        
        updateControlText(`#channel${i}title`, episodeList[i].title.replace("(", "<nobr>(").replace(")", ")</nobr>"));
                
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
    }

    // Update clock
    var now = new Date();
    var currentTime = days[now.getDay()] + ' ' + ('0' + now.getDate()).slice(-2) + ' ' + months[now.getMonth()] + 
        ' ' + ('0' + now.getHours()).slice(-2) + ':' + ('0' + now.getMinutes()).slice(-2);

    $("#currentTime").text(currentTime);
}

function updateControlText(control, value) {  
    $(control).html(value);
    //if(document.getElementById(control.substring(1)) && document.getElementById(control.substring(1)).innerHTML != value) {
    //    $(control).fadeOut(function() {            
    //        $(this).html(value)
    //    }).fadeIn();
    //}
}

function readyToPlayVideo(event) { 
    videoContainer.ready = true;
    console.log('Episode ready to play, total length ' + video.duration);
    console.log('Episode title', episodeList[selectedChannel].title);
}

function playPauseClick() {
    if(videoContainer.ready) {
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

            // Switch settings off if enabled
            settingsVisible = false;
            
            $('#settingsToggleIcon').removeClass("bi-info-circle-fill");
            $('#settingsToggleIcon').addClass("bi-info-circle");
    
            $('#channelList').find('.showDetails').removeClass('initially-hidden');
            $('#channelList').find('.channelDetails').addClass('initially-hidden');     
        }
    }
}

// Wire up channel selection buttons
for(let i=1; i<=totalChannels; i++) {
    $(`#channel${i}button`).on('click', function(event) {
        switchChannel(i);
        event.preventDefault();
    });
    $(`#channel${i}title`).on('click', function(event) {
        switchChannel(i);
        event.preventDefault();
    });
    $(`#channel${i}description`).on('click', function(event) {
        switchChannel(i);
        event.preventDefault();
    });
    $(`#settings${i}title`).on('click', function(event) {
        switchChannel(i);
        event.preventDefault();
    });
    $(`#settings${i}description`).on('click', function(event) {
        switchChannel(i);
        event.preventDefault();
    });
};

// Wire up volume mute
$("#speakerToggle").on('click', function(event) {
    if(videoContainer.video.muted) {
        $("#speakerIcon").removeClass("bi-volume-mute-fill");
        $("#speakerIcon").addClass("bi-volume-up-fill");
    }
    else {
        $("#speakerIcon").removeClass("bi-volume-up-fill");
        $("#speakerIcon").addClass("bi-volume-mute-fill");
    }
    videoContainer.video.muted = !videoContainer.video.muted;
    return false;
})

// Wire up settings toggle
$("#settingsToggle").on('click', function(event) {
    settingsVisible = !settingsVisible;
    
    if(settingsVisible) {
        $('#settingsToggleIcon').removeClass("bi-info-circle");
        $('#settingsToggleIcon').addClass("bi-info-circle-fill");

        $('#channelList').find('.showDetails').addClass('initially-hidden');
        $('#channelList').find('.channelDetails').removeClass('initially-hidden');
    } else {
        $('#settingsToggleIcon').removeClass("bi-info-circle-fill");
        $('#settingsToggleIcon').addClass("bi-info-circle");

        $('#channelList').find('.showDetails').removeClass('initially-hidden');
        $('#channelList').find('.channelDetails').addClass('initially-hidden');
    }
    
    return false;
});

// Wire up iFrame listener
window.onmessage = function(e) {
    if (e.data == 'click') {
        playPauseClick();
    }
};

// Wire up channel group toggle
$("#channelGroupToggle").on('click', function(event) {
     // Cycle channel groupings
     $(`#channelGrouping${visibleChannelGroup}`).addClass('initially-hidden');
     $(`#channelGroup${visibleChannelGroup}button`).removeClass(`bi-${visibleChannelGroup}-square-fill`);
     $(`#channelGroup${visibleChannelGroup}button`).addClass(`bi-${visibleChannelGroup}-square`);
     $(`#channelSettings${visibleChannelGroup}`).addClass('initially-hidden');
      
     visibleChannelGroup++;
     if(visibleChannelGroup === 3) {
         visibleChannelGroup = 1;
     }

     $(`#channelGrouping${visibleChannelGroup}`).removeClass('initially-hidden');
     $(`#channelSettings${visibleChannelGroup}`).removeClass('initially-hidden');
     $(`#channelGroup${visibleChannelGroup}button`).removeClass(`bi-${visibleChannelGroup}-square`);
     $(`#channelGroup${visibleChannelGroup}button`).addClass(`bi-${visibleChannelGroup}-square-fill`);

    return false;
});

// Wire up video canvas event listeners
videoCanvas.addEventListener("click", playPauseClick);
videoCanvas.addEventListener("ended", advanceEpisode);
