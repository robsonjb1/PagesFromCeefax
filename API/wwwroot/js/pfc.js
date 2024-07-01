(function () {
    // Page initialisation
    $(window).on("load", function () {

    // Canvas
    const canvas = document.querySelector('canvas');
    canvas.width = 480; 
    canvas.height = 500;
    
    const ctx = canvas.getContext('2d');
    var imgData = ctx.createImageData(canvas.width, canvas.height);

    canvas.addEventListener('click', function(ev) {
        var {x, y} = getCursorPosition(canvas, ev);
        canvasWidth = $('#magazineCanvas').width();
        canvasHeight = $('#magazineCanvas').height();

        if(y > canvasHeight * 0.95) // User as selected the bottom row
        {        
            if(x < canvasWidth * 0.3) ToggleMusic();                                                    // Red
            if(x >= canvasWidth * 0.3 && x < canvasWidth * 0.6) { debugOffset--; MoveNextPage(); }      // Green
            if(x >= canvasWidth * 0.6 && x < canvasWidth * 0.9) { debugOffset++; MoveNextPage(); }      // Yellow
            if(x >= canvasWidth * 0.9) window.open("https://x.com/pfceefax");                           // Cyan
        }
    });

    // Carousel state
    var currentSeconds = transitionSecond = -1;
    var magazineRequestTime;
    var currentPage = pagesInCarousel = debugOffset = 0;
    
    var loadingPageDuration = 10;
    var pageDuration = 27;
    var pageTickerNo = 100;
    var pageTicking = true;
    var lastPageRefresh = new Date();
    var pageIsValid = carouselIsValid = true; 
    var pageBuffer = getLoadingPage();  // getTestPage();
    var carousel = null;
    var musicOn = false;

    // Render state
    var prevCol = bgCol = holdChar = 0;
    var col = 7;
    var sep = holdOff = gfx = heldChar = false;
    var dbl = oldDbl = secondHalfOfDouble = wasDbl = false;
    var flash = flashOn = false;
    var flashTime = rowLimit = 1;
    var lastRowRefresh = false;
    
    // Character definitions
    var charSmoothed = makeSmoothedChars(getChars());
    var charGraphics = makeGraphicChars(getChars(), false);
    var charSeparated = makeGraphicChars(getChars(), true);
  
    // Functions
    function render(time) {
        if(!lastRowRefresh || rowLimit != 25 || time - lastRowRefresh >= 250) { // Refresh screen every quarter second
            // Change page if we've hit the transition second, or it has been at least a minute since the last refresh
            var now = new Date();
            currentSeconds = now.getSeconds();

            // Do we need to refresh the magazine ? (takes place every 30 minutes)
            if (now - magazineRequestTime > (30 * 60 * 1000)) {
                magazineRequestTime = now;
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
                MoveNextPage();
            }

            // Determine number of rows to draw
            lastRowRefresh = time;
            rowLimit = rowLimit >= 25 ? 25 : rowLimit+2;
            
            // Increment ticker with a random delay to make less uniform
            if(pageTicking && rowLimit >= 25) pageTickerNo = (pageTickerNo > 190) ? 100 : pageTickerNo + ((Math.floor((Math.random() * 10)) == 1) ? 2 : 1); 
            
            var charPos = 0;
            if (++flashTime === 8) flashTime = 0;  // 3:1 flash ratio.
            flashOn = flashTime < 2;

            // Render page, initialise header and double height flags
            insertPageHeader(carouselIsValid, pageIsValid, pageBuffer, pageTickerNo, musicOn);
            dbl = oldDbl = secondHalfOfDouble = wasDbl = false;

            for(var yCol = 0; yCol < rowLimit; yCol++)
            {
                col = 7;
                bg = 0;
                holdChar = false;
                heldChar = 0x20;
                nextGlyphs = heldGlyphs = charSmoothed;
                sep = gfx = dbl = flash = false;
                secondHalfOfDouble = secondHalfOfDouble ? false : wasDbl;
                wasDbl = false;
                
                for(var xCol = 0; xCol < 40; xCol++)
                {
                    idPtr = (xCol * 4 * 12) + (yCol * 40 * 4 * 12 * 20);
                    oldDbl = dbl;
                    prevCol = col;
                    curGlyphs = nextGlyphs;
                    prevFlash = flash;
                    data = pageBuffer[charPos++];

                    if (data < 0x20) {
                        data = handleControlCode(data);
                    } else if (gfx) {
                        heldChar = data;
                        heldGlyphs = curGlyphs;
                    } else if (data == 128) // A marked invalid character (will flag invalid page)
                    {
                        data = 63; // Display as question mark (?)
                    }

                    // Displayable character, map to character definitions
                    var charDef = (prevFlash && flashOn) || (secondHalfOfDouble && !dbl) ? 0 : data - 32;
                    for(var yPos = 0; yPos < 20; yPos++)
                    {
                        actualY = dbl ? Math.floor(yPos / 2) + (secondHalfOfDouble ? 10 : 0) : yPos;
                        for(var xPos = 0; xPos < 12; xPos++)
                        {
                            let setPixel = curGlyphs[(charDef * 240) + (12 * actualY) + xPos] == 1;
                            imgData.data[idPtr] = setPixel ? getRGB_Red(prevCol) : getRGB_Red(bg);
                            imgData.data[idPtr+1] = setPixel ? getRGB_Green(prevCol) : getRGB_Green(bg);
                            imgData.data[idPtr+2] = setPixel ? getRGB_Blue(prevCol) : getRGB_Blue(bg);
                            imgData.data[idPtr+3] = setPixel || (!setPixel && bg != 0) ? 255 : 0;
                            idPtr = idPtr + 4;
                        }
                        
                        idPtr = idPtr + (4 * 39 * 12);      
                    }
                    
                    if (holdOff) {
                        holdChar = false;
                        heldChar = 32;
                    }
                }
            }

            ctx.putImageData(imgData, 0, 0);
        }

        requestAnimationFrame(render);
    }

    function setNextChars()
    {
        if (gfx) {
            if (sep) {
                nextGlyphs = charSeparated;
            } else {
                nextGlyphs = charGraphics;
            }
        } else {
            nextGlyphs = charSmoothed;
        }
    }

    function handleControlCode(data)
    {
        holdOff = false;
        switch (data) 
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                gfx = false;
                col = data;
                setNextChars();
                break;
            case 8:
                flash = true;
                break;
            case 9:
                flash = false;
                break;

            case 12:
            case 13:
                dbl = !!(data & 1);
                if (dbl) wasDbl = true;
                break;

            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
                gfx = true;
                col = data & 7;
                setNextChars();
                break;
            case 24:
                col = prevCol = bg;
                break;
            case 25:
                sep = false;
                setNextChars();
                break;
            case 26:
                sep = true;
                setNextChars();
                break;
            case 28:
                bg = 0;
                break;
            case 29:
                bg = col;
                break;
            case 30:
                holdChar = true;
                break;
            case 31:
                holdOff = true;
                break;
        }

        if (holdChar && dbl === oldDbl) {
            data = heldChar;
            if (data >= 0x40 && data < 0x60) data = 0x20;
            curGlyphs = heldGlyphs;
        } else {
            heldChar = 0x20;
            data = 0x20;
        }

        return data;
    }

    function getNewCarousel() {
        // Mark the time we first requested the magazine
        magazineRequestTime = new Date();
       
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
                var actualWait = (magazineReceiveTime - magazineRequestTime) / 1000;
                if (actualWait >= loadingPageDuration) {
                    // More than 15 seconds have passed, so move to the first page straight away
                    transitionSecond = (magazineReceiveTime.getSeconds() + 1) % 60;
                }
                else {
                    // Magazine received before 15 seconds have passed, so calculate the correct transition second
                    transitionSecond = (magazineRequestTime.getSeconds() + loadingPageDuration) % 60;
                }
            }
        });
    }

    function MoveNextPage() {
        var now = new Date();
        transitionSecond = (now.getSeconds() + pageDuration) % 60;
        lastPageRefresh = now;
        currentPage = (debugOffset + (Math.floor(((now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) / pageDuration))) % pagesInCarousel;
        pageBuffer = carousel.content[currentPage].data;
        pageIsValid = carousel.content[currentPage].isValid;
        rowLimit = 1; // Force new page redraw
        for(var i=3+(40*20*4); i<40*20*12*24*4; i+=4) imgData.data[i] = 0;   // Blank the screen (only lines 2 to 24, keep header and Fastext buttons)
    }

    // Event handlers
    window.onkeydown = function(key) {  
        if(key.keyCode === 37) {  
            debugOffset--;          // Left arrow
            MoveNextPage();
        };  
        if(key.keyCode === 38) {  
            debugOffset = 0;        // Up arrow
            MoveNextPage();
        };  
        if(key.keyCode === 39) {  
            debugOffset++;          // Right arrow
            MoveNextPage();
        };  
    }

    // Support for background music
    function ToggleMusic() {
        var now = new Date();
        var minutes = now.getMinutes();
        var hours = now.getHours();
        var seconds = now.getSeconds();
        var month = now.getMonth();

        // Select track and position
        var track = 0;
        position = ((hours * 60 * 60) + (minutes * 60) + seconds) % (3 * 60 * 60);      // Full track is 3 hours long

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

    // Start the emulation
    requestAnimationFrame(render);
    getNewCarousel();   
  });
  })();