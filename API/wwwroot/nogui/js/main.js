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

    // Carousel state
    var currentSeconds = -1
    var transitionSecond = -1;
    var rowLimit = 0;
    var lastRowRefresh = false;
    var currentPage = 0
    var loadingPageDuration = 10;
    var pageDuration = 27;
    var pageTickerNo = 100;
    var pageTicking = true;
    var lastPageRefresh = new Date();
    var pageIsValid = true;
    var pagesInCarousel = 0
    var carousel = null;
    var carouselRequestTime;
    var carouselIsValid = true; 
    var pageBuffer = getLoadingPage();  // getTestPage();
  
    // Functions
    function timerTrigger(time) {
        if(!lastRowRefresh || rowLimit != 27 || time - lastRowRefresh >= 250) { // Refresh screen every quarter second
            // Change page if we've hit the transition second, or it has been at least a minute since the last refresh
            var now = new Date();
            currentSeconds = now.getSeconds();

            // Do we need to refresh the magazine ? (takes place every 30 minutes)
            if (now - carouselRequestTime > (30 * 60 * 1000)) {
                carouselRequestTime = now;
                lastPageRefresh = now;
                transitionSecond = -1; // do not do a regular refresh until the new carousel is loaded
              
                // Switch back to loading page and turn on page ticker
                pageBuffer = getLoadingPage();
                pageTicking = true;
                rowLimit = 1; // Force new page redraw    

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
            rowLimit = rowLimit >= 27 ? 27 : rowLimit+2;
            
            // Increment ticker with a random delay to make less uniform
            if(pageTicking && rowLimit == 27) pageTickerNo = (pageTickerNo > 190) ? 100 : pageTickerNo + ((Math.floor((Math.random() * 10)) == 1) ? 2 : 1); 
            
            // Render page
            insertPageHeader(carouselIsValid, pageIsValid, pageBuffer, pageTickerNo);
            screen.redraw(ctx, pageBuffer, imgData, (rowLimit == 27 ? 1 : rowLimit));
        }

        requestAnimationFrame(timerTrigger);
    }

    function insertPageHeader(carouselIsValid, pageIsValid, charData, pageCycle)
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
    }
   
    function getNewCarousel() {
        // Mark the time we first requested the magazine
        carouselRequestTime = new Date();
       
        // Request the carousel
        $.ajax ({
            dataType: 'json',
            url: "../carousel",
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
        currentPage = (Math.floor(((now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) / pageDuration)) % pagesInCarousel;
        pageBuffer = carousel.content[currentPage].data;
        pageIsValid = carousel.content[currentPage].isValid;
        rowLimit = 1; // Force new page redraw
        for(var i=3+(40*20*4); i<40*20*12*24*4; i+=4) imgData.data[i] = 0;   // Blank the screen (only lines 2 to 24, keep header and Fastext buttons)
    }

    // Start the emulation
    requestAnimationFrame(timerTrigger);
    getNewCarousel();   
  });
  })();