import { video } from "./video.js";

(function () {
    // Page initialisation
    $(window).on("load", function () {

    // Canvas
    const canvas = document.querySelector('canvas');
    canvas.width = 480; 
    canvas.height = 500;
    
    var screen = new video();
    const ctx = canvas.getContext('2d');
    var imgData = ctx.createImageData(canvas.width, canvas.height);

    canvas.addEventListener('click', function(ev) {
        var {x, y} = getCursorPosition(canvas, ev);
        var canvasWidth = $('#magazineCanvas').width();
        var canvasHeight = $('#magazineCanvas').height();

        if(y > canvasHeight * 0.90) // User as selected the bottom row
        {        
            if(x < canvasWidth * 0.35) toggleMusic();                                                    // Red
            if(x >= canvasWidth * 0.35 && x < canvasWidth * 0.7) { debugOffset--; moveNextPage(); }      // Green
            if(x >= canvasWidth * 0.7 && x < canvasWidth * 0.9) { debugOffset++; moveNextPage(); }      // Yellow
            if(x >= canvasWidth * 0.9) toggleHold();                                                   // Cyan
        }
    });

    // Carousel state
    var currentSeconds = -1
    var transitionSecond = -1;
    var debugOffset = 0;
    var rowLimit = 0;
    var lastRowRefresh = false;
    var currentPage = 0
    var loadingPageDuration = 8;
    var pageDuration = 30;
    var pageTickerNo = 100;
    var pageTicking = true;
    var lastPageRefresh = new Date();
    var pageIsValid = true;
    var pagesInCarousel = 0
    var carousel = null;
    var carouselRequestTime;
    var carouselIsValid = true; 
    var pageBuffer = getLoadingPage();  // getTestPage();
    var musicOn = false;
    var holdPage = false;
  
    // Functions
    function timerTrigger(time) {
        if(!lastRowRefresh || rowLimit != 25 || time - lastRowRefresh >= 250) { // Refresh screen every quarter second
            // Change page if we've hit the transition second, or it has been at least a minute since the last refresh
            var now = new Date();
            currentSeconds = now.getSeconds();

            // If in freeze mode, silently request a new carousel every 5 minutes
            if(holdPage && now - carouselRequestTime > (5 * 60 * 1000)) {
                carouselRequestTime = now;
                lastPageRefresh = now;
                transitionSecond = -1; // do not do a regular refresh until the new carousel is loaded

                // Refresh magazine
                getNewCarousel();
            }
            
            // If not, do we need to refresh the magazine ? (takes place every 25 minutes)
            if(!holdPage && (now - carouselRequestTime > (25 * 60 * 1000))) {
                carouselRequestTime = now;
                lastPageRefresh = now;
                transitionSecond = -1; // do not do a regular refresh until the new carousel is loaded
            
                // Switch back to loading page and turn on page ticker
                pageBuffer = getLoadingPage();
                pageTicking = true;
                rowLimit = 0; // Force new page redraw    

                // Refresh magazine
                getNewCarousel();
            }

            if (currentSeconds == transitionSecond || (now - lastPageRefresh > 60 * 1000)) {
                // Turn off the ticker
                pageTicking = false;
                pageTickerNo = 152;

                // Move to next page
                moveNextPage();
            }

            // Determine number of rows to draw
            lastRowRefresh = time;
            rowLimit = rowLimit >= 25 ? 25 : rowLimit+2;
            
            // Increment ticker with a random delay to make less uniform
            if(pageTicking && rowLimit >= 25) pageTickerNo = (pageTickerNo > 190) ? 100 : pageTickerNo + ((Math.floor((Math.random() * 10)) == 1) ? 2 : 1); 
                        
            // Render page
            insertPageHeader(carouselIsValid, pageIsValid, pageBuffer, pageTickerNo, musicOn);
            screen.redraw(ctx, pageBuffer, imgData, rowLimit);
        }

        requestAnimationFrame(timerTrigger);
    }

    function insertPageHeader(carouselIsValid, pageIsValid, charData, pageCycle, musicOn)
    {
        var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        var now = new Date();
    
        const header = ' P152   CEEFAX 1 ' + pageCycle.toString() + ' ' + days[now.getDay()] + ' ' + ('0' + now.getDate()).slice(-2) + ' ' + months[now.getMonth()] + 
            String.fromCharCode(3) +
            ('0' + now.getHours()).slice(-2) + ':' + ('0' + now.getMinutes()).slice(-2) + '/' + ('0' + now.getSeconds()).slice(-2);
        
        for(var i=0; i<header.length; i++) charData[i] = header.charCodeAt(i);
        
        charData[0] = pageIsValid ? 32 : 1; // Set red page number if invalid
        charData[5] = carouselIsValid ? 7 : 1; // Set red page number if invalid
        charData[16] = 7; // Set white text for the remainder

        // Insert Fastext buttons
        const footer = " Music:" + (musicOn ? "On " : "Off") + "   Previous   Next    Hold:" + (holdPage ? "On " : "Off");
        
        for(var i=0; i<footer.length; i++) charData[960+i] = footer.charCodeAt(i);
        charData[960] = 1;  // Red
        charData[970] = 2;  // Green
        charData[982] = 3;  // Yellow
        charData[989] = 6;  // Cyan
        charData[972] = 0x5b; // Left arrow
        charData[988] = 0x5d; // Right arrow
    }
   
    function getNewCarousel() {
        // Mark the time we first requested the magazine
        carouselRequestTime = new Date();
       
        // Request the carousel
        $.ajax ({
            dataType: 'json',
            url: "./carousel",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                carousel = data.carousel;
                carouselIsValid = carousel.isValid;
                pagesInCarousel = carousel.totalPages;
                
                var magazineReceiveTime = new Date();
                var actualWait = (magazineReceiveTime - carouselRequestTime) / 1000;
                if (actualWait >= loadingPageDuration) {
                    // More than 15 seconds have passed, so move to the first page straight away
                    transitionSecond = (magazineReceiveTime.getSeconds() + 1) % 60;
                }
                else {
                    // Magazine received before 15 seconds have passed, so calculate the correct transition second
                    transitionSecond = (carouselRequestTime.getSeconds() + loadingPageDuration) % 60;
                }
            }
        });
    }

    function moveNextPage() {
        var now = new Date();
        transitionSecond = (now.getSeconds() + pageDuration) % 60;
        lastPageRefresh = now;
        
        // force new page redraw
        rowLimit = 0;
        
        if(!holdPage)   // If holding a page, do not recalculate the current page or blank the screen, just keep showing the current page
        {
            currentPage = (debugOffset + (Math.floor(((now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) / pageDuration))) % pagesInCarousel;
            for(var i=3+(40*20*4); i<40*20*12*24*4; i+=4) imgData.data[i] = 0;   // Blank the screen (only lines 2 to 24, keep header and Fastext buttons)
        }

        pageBuffer = carousel.content[currentPage].data;
        pageIsValid = carousel.content[currentPage].isValid;
    }

    function getCursorPosition(canvas, ev) {
        var rect = canvas.getBoundingClientRect()
        var x = ev.clientX - rect.left
        var y = ev.clientY - rect.top
        return {x, y};
    }
   
    // Support for background music
    function toggleMusic() {
        var now = new Date();
        var minutes = now.getMinutes();
        var hours = now.getHours();
        var seconds = now.getSeconds();
        var month = now.getMonth();

        // Select track and position
        var track = 0;
        var position = ((hours * 60 * 60) + (minutes * 60) + seconds) % (3 * 60 * 60);  // Full track is 3 hours long

        if (month == 11) { // December for Christmas
            track = 1;
            position = ((hours * 60 * 60) + (minutes * 60) + seconds) % (1 * 60 * 60);  // Full track is 1 hour long
        }
        if (!musicOn) {
            $('audio')[track].play();
            $('audio')[track].currentTime = position;
        }
        else {
            $('audio')[track].pause();
        }

        musicOn = !musicOn;
    }

    // Togle hold state
    function toggleHold() {
        holdPage = !holdPage;
    }

    // Event handlers
    window.onkeydown = function(key) {  
        if(key.keyCode === 37) {  
            debugOffset--;          // Left arrow
            moveNextPage();
        };  
        if(key.keyCode === 38) {  
            debugOffset = 0;        // Up arrow
            moveNextPage();
        };  
        if(key.keyCode === 39) {  
            debugOffset++;          // Right arrow
            moveNextPage();
        };  
    }

    // Start the emulation
    requestAnimationFrame(timerTrigger);
    getNewCarousel();   
  });
  })();