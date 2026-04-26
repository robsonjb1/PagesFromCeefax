// Initialisation
let totalChannels = 10;
let selectedChannel = 0;
let showIdentNext = true;
let settingsVisible = false;
let channelsVisible = true;
let channelSelections = "";
let lastBannerRefresh = new Date();
var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

// Retrieve channel selection cookie if present
if(document.cookie) {
    channelSelections = document.cookie.substring(document.cookie.indexOf("=") + 1)
}
if(channelSelections == "" || channelSelections.indexOf("|") == -1) {
    channelSelections = "|1||2||9||10|";
}

// Enable channel settings
for(var c=1; c<=totalChannels; c++) {   
    var test = "|" + c.toString() + "|";
    if(channelSelections.indexOf(test) > -1) {
        $(`#settings${c}icon`).addClass("bi-toggle-on");
        $(`#settings${c}icon`).removeClass("bi-toggle-off");        
        $(`#channel${c}row`).show();

        if(selectedChannel == 0) {
            selectedChannel = c; // Use this as initial channel
        }
    } else {
        $(`#settings${c}icon`).addClass("bi-toggle-off");
        $(`#settings${c}icon`).removeClass("bi-toggle-on");    
        $(`#channel${c}row`).hide();
    }
}

// Initialise episode list
let episodeList = new Array(totalChannels+1);
for(var c=0; c<=totalChannels; c++) {
    episodeList[c] = {};
}

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
setTimeout(autoRefreshChannelFeeds, 600000);    // 10 minutes
setTimeout(autoRefreshCaption, 10000);          // 10 seconds
switchChannel(selectedChannel);


function refreshChannelFeeds() {
    console.log("Refreshing channel feeds...");
    
    $.ajaxSetup({
        async: false
    });

    for(var i=1; i<=totalChannels; i++) {
        $.getJSON(`./js/cat-channel-${i}.json?z=${Math.random()}`, function(json) {
            episodeList[i].data = json.episodeList;
            episodeList[i].lastRefresh = json.lastRefresh;
        });
    }

    // Ping the PFC API for logging purposes
    $.ajaxSetup({
        async: true
    });
    $.getJSON(`../ping/${selectedChannel}`, function(json) { });
}

function autoRefreshCaption() {
    let now = new Date();
    let refreshChannel = (now - lastBannerRefresh) > 30000
    lastBannerRefresh = now;

    setTimeout(autoRefreshCaption, 10000); // If the banner is showing, update the content every 10 seconds

    if(refreshChannel)
    {
        // More than 30 seconds since the last banner refresh (it should be 10 seconds)
        // Reselect the channel to ensure we are at the right show/time
        switchChannel(selectedChannel);
        console.log('Resyncing channel...');
    }
    
    if($('#planner').is(":visible")) {
        updateChannelStats();
        displayCaption();
    }
}

function autoRefreshChannelFeeds() {
    setTimeout(autoRefreshChannelFeeds, 600000); // Update the channel feeds every 10 minutes
    refreshChannelFeeds();
}

function switchSettings(channel) {
    var sChannel = channel.toString();

    if(channelSelections.indexOf("|" + sChannel + "|") == -1) {
        $(`#channel${channel}row`).show();
        $(`#settings${channel}icon`).removeClass("bi-toggle-off");    
        $(`#settings${channel}icon`).addClass("bi-toggle-on");    
        channelSelections += "|" + sChannel + "|";
    } else {
        $(`#channel${channel}row`).hide();
        $(`#settings${channel}icon`).removeClass("bi-toggle-on");    
        $(`#settings${channel}icon`).addClass("bi-toggle-off");  
        channelSelections = channelSelections.replace("|" + sChannel + "|", "");
    }
    document.cookie = `channelSelections=${channelSelections}; expires=Thu, 1 Jan 2029 12:00:00 UTC; path=/`;
}

function switchChannel(channel) {
    showIdentNext = true;
    var pfcAlreadyDisplayed = episodeList[selectedChannel].isMusic || episodeList[selectedChannel].isPodcast;
    
    // Update stats for each channel to ensure the banner is accurate
    updateChannelStats();
   
    $(`#channel${selectedChannel}button`).removeClass('active');
    selectedChannel = channel;
    $(`#channel${selectedChannel}button`).addClass('active');

    // Show/hide PFC overlay
    if(!pfcAlreadyDisplayed && (episodeList[selectedChannel].isMusic || episodeList[selectedChannel].isPodcast)) {
        $('#pfcFrame').attr("src", "../");
        $('#pfcOverlay').show();
    }
    if(!(episodeList[selectedChannel].isMusic || episodeList[selectedChannel].isPodcast)) {
        $('#pfcFrame').attr("src", "");
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
        let hours = Math.floor((totalTimes - (days * 86400)) / 3600, 1)
  
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

                break;
            }
            else {
                sum += episodeList[channel].data[i].length;
            }
        }

        // Display the currently playing show on the settings table
        let hoursPrompt = "hours";
        if(hours == 1) {
            hoursPrompt = "hour";
        }

        if(days != 0) {
            $(`#settings${channel}description`).text(`${episodeList[channel].lastRefresh} (${episodeList[channel].data.length} items, ${days} days ${hours} ${hoursPrompt})`); 
        }
        else {
            $(`#settings${channel}description`).text(`${episodeList[channel].lastRefresh} (${episodeList[channel].data.length} items, ${hours} ${hoursPrompt})`); 
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
        $(`#channel${i}title`).html(episodeList[i].title.replace("(", "<nobr>(").replace(")", ")</nobr>"));
                
        // If >85% through show up next rather than the description. Radio channels always show description
        var upNext = "(at " + episodeList[i].endTime + ") " + episodeList[i].upNext;
        
        if(!episodeList[i].isMusic && (episodeList[i].description == "" || episodeList[i].description.indexOf(" ") == -1 || episodeList[i].perc >= 85)) {
            $(`#channel${i}description`).html(upNext);
        }
        else {
            $(`#channel${i}description`).html(episodeList[i].description.replace("(", "<nobr>(").replace(")", ")</nobr>"));
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
            if(settingsVisible) {
                // If you've clicked on the video whilst the settings was open, close settings and go back to the channel list in the background
                settingsVisible = false;
                channelsVisible = true;

                $('#settingsToggleIcon').removeClass("bi-gear-fill");
                $('#settingsToggleIcon').addClass("bi-gear");

                $("#planner").hide();
                $("#settings").hide();
                $("#channelList").fadeIn();
            }
            else {
                // Update stats for each channel
                updateChannelStats();
                displayCaption();
                $("#planner").fadeToggle();
            } 
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
    if(channelsVisible) {
        settingsVisible = !settingsVisible;
        $("#settings").toggle();
        $("#channelList").toggle();

        if(settingsVisible) {
            $('#settingsToggleIcon').removeClass("bi-gear");
            $('#settingsToggleIcon').addClass("bi-gear-fill");
        } else {
            $('#settingsToggleIcon').removeClass("bi-gear-fill");
            $('#settingsToggleIcon').addClass("bi-gear");
        }
    }

    // Allow the channel numbers to show in the planner after we've visited the channel list page
    $('#channelList').find('.inTitleChannel').removeClass('initially-hidden');

    return false;
});

// Wire up settings selector buttons
for(let i=1; i<=totalChannels; i++) {
    $(`#settings${i}button`).on('click', function(event) {
        switchSettings(i);
        event.preventDefault();
    });
    $(`#settings${i}title`).on('click', function(event) {
        switchSettings(i);
        event.preventDefault();
    });
    $(`#settings${i}description`).on('click', function(event) {
        switchSettings(i);
        event.preventDefault();
    });
};

// Wire up iFrame listener
window.onmessage = function(e) {
    if (e.data == 'click') {
        playPauseClick();
    }
};

// Wire up page width toggle
$("#castVideo").on('click', function(event) {
    videoContainer.video.pause();
    window.open(videoContainer.video.src);

    return false;
});

// Wire up reverse channels switch
$("#reverseChannels").on('click', function(event) {
    if($('#reverseChannelsIcon').hasClass('bi-layout-sidebar-inset')) {
        $('#reverseChannelsIcon').removeClass('bi-layout-sidebar-inset');
        $('#reverseChannelsIcon').addClass('bi-layout-sidebar-inset-reverse');
    } else {
        $('#reverseChannelsIcon').removeClass('bi-layout-sidebar-inset-reverse');
        $('#reverseChannelsIcon').addClass('bi-layout-sidebar-inset');
    }

    var newChannelSelections = "";

    for(var c=1; c<=totalChannels; c++) {   
        var test = "|" + c.toString() + "|";
        if(channelSelections.indexOf(test) == -1) {
            $(`#settings${c}icon`).addClass("bi-toggle-on");
            $(`#settings${c}icon`).removeClass("bi-toggle-off");        
            $(`#channel${c}row`).show();

            newChannelSelections += test;
        } else {
            $(`#settings${c}icon`).addClass("bi-toggle-off");
            $(`#settings${c}icon`).removeClass("bi-toggle-on");    
            $(`#channel${c}row`).hide();
        }
    }

    channelSelections = newChannelSelections;
    document.cookie = `channelSelections=${channelSelections}; expires=Thu, 1 Jan 2029 12:00:00 UTC; path=/`;
});

// Wire up video canvas event listeners
videoCanvas.addEventListener("click", playPauseClick);
videoCanvas.addEventListener("ended", advanceEpisode);
