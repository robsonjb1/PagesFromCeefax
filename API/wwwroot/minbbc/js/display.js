import * as chardefs from "./chardefs.js";

export class Display {
    
    constructor()
    {
        // Character definitions
        this.charSmoothed = this.makeSmoothedChars(chardefs.getChars());
        this.charGraphics = this.makeGraphicChars(chardefs.getChars(), false);
        this.charSeparated = this.makeGraphicChars(chardefs.getChars(), true);

        // Render state, preserved between redraw calls
        this.prevCol = 0;
        this.bg = 0;
        this.holdChar = 0;
        this.col = 7;
        this.sep = false;
        this.holdOff = false;
        this.gfx = false
        this.heldChar = false;
        this.dbl = false;
        this.oldDbl = false;
        this.secondHalfOfDouble = false;
        this.wasDbl = false;
        this.flash = false;
        this.flashOn = false;
        this.flashTime = -1;
        this.prevFlash = false; 
        this.lastRowRefresh = false;
        this.nextGlyphs = this.charSmoothed;
        this.heldGlyphs = this.charSmoothed;  
        this.curGlyphs = this.charSmoothed;    

        this.actual2bitColour = new Uint8Array([0, 2, 8, 10]); 
    }

    // Teletext rendering utilities
    getRGB_Red(col)
    {
        switch(col)
        {
            case 1:
            case 3:
            case 5:
            case 7:
                return 255;
            default: 
                return 0;
        }
    }

    getRGB_Green(col)
    {
        switch(col)
        {
            case 2:
            case 3:
            case 6:
            case 7:
                return 255;
            default: 
                return 0;
        }
    }

    getRGB_Blue(col)
    {
        switch(col)
        {
            case 4:
            case 5:
            case 6:
            case 7:
                return 255;
            default: 
                return 0;
        }
    }

    makeSmoothedChars(charData)
    {
        // Convert the original 6x10 matrix to 12x20 and apply smoothing algorithm
        var smoothedData = new Uint8Array(12 * 20 * 96);

        for(var charNo=0; charNo<96; charNo++)
        {
            for(var row=0; row<10; row++)
            {
                for(var pixel=0; pixel<7; pixel++)
                {
                    var sourcePixel = charData[(charNo * 60) + (row * 6) + pixel];
                    var destPos = (charNo * 240) + (row * 24) + (pixel * 2);

                    this.solidBlock(smoothedData, destPos, 2, 2, sourcePixel, false);
                }
            }

            // Smooth the character
            for(var row=0; row<19; row++)
            {
                for(var pixel=0; pixel<11; pixel++)
                {
                    var sourcePos = (charNo * 240) + (row * 12) + pixel;
                    
                    // Detect a diagonal
                    if((smoothedData[sourcePos] == 1                 
                        && smoothedData[sourcePos+1] == 0           // 1 0
                        && smoothedData[sourcePos+12] == 0          // 0 1
                        && smoothedData[sourcePos+13] == 1          
                    ) || (smoothedData[sourcePos] == 0                 
                        && smoothedData[sourcePos+1] == 1           // 0 1
                        && smoothedData[sourcePos+12] == 1          // 1 0
                        && smoothedData[sourcePos+13] == 0          
                    )) {
                        this.solidBlock(smoothedData, sourcePos, 2, 2, 1, false);
                    }
                }
            }
        }

        return smoothedData;
    }

    solidBlock(graphicData, startPos, width, height, val, sep)
    {
        for(var xPos = 0; xPos<width; xPos++)
        {
            for(var yPos=0; yPos<height; yPos++)
            {
                // Separated graphics miss the left and bottom sides
                graphicData[startPos + xPos + (12 * yPos)] = val && (!sep || xPos > 1) && (!sep || yPos<height-2) ? 1 : 0;
            }
        }
    }

    makeGraphicChars(charData, sep)
    {
        // Construct the graphic data using the smoothed chars as a template (as we need to preserve the CAPS alpha chars)
        var graphicData = this.makeSmoothedChars(charData);

        for(var char=0; char<64; char++)
        {
            var startPos = (char * 12 * 20) + (char > 31 ? (32 * 12 * 20) : 0);             // Add offset to skip over the CAPS alpha chars

            this.solidBlock(graphicData, startPos, 6, 6, !!(char & 1), sep);                     // Top left
            this.solidBlock(graphicData, startPos + 6, 6, 6, !!(char & 2), sep);                 // Top right
            this.solidBlock(graphicData, startPos + (12 * 6), 6, 8, !!(char & 4), sep);          // Middle left, row is larger at 8 pixels
            this.solidBlock(graphicData, startPos + (12 * 6) + 6, 6, 8, !!(char & 8), sep);      // Middle right, row is larger at 8 pixels
            this.solidBlock(graphicData, startPos + (12 * 14), 6, 6, !!(char & 16), sep);        // Bottom left
            this.solidBlock(graphicData, startPos + (12 * 14) + 6, 6, 6, !!(char & 32), sep);    // Bottom right
        }

        return graphicData;
    }

    getRed(pal)
    {
        return (pal & 1) === 0 ? 255 : 0;
    }

    getGreen(pal)
    {
        return (pal & 2) === 0 ? 255 : 0;
    }

    getBlue(pal)
    {
        return (pal & 4) === 0 ? 255 : 0;
    }

    getScreenMode(processor)
    {
        return (processor.readmem(0x355));
    }

    getScreenStart(screenMode)
    {
        switch(screenMode)
        {
            case 0:
            case 1:
            case 2: 
                return 0x3000;
            case 3:
                return 0x4000;
            case 4:
            case 5:
                return 0x5800;
            case 6:
                return 0x6000;
        }
    }

    getScreenOffset(screenMode)
    {
        switch(screenMode)
        {
            case 0:
            case 1:
            case 2:
                return 0x600;
            case 3:
                return 0x800;
            case 4:
            case 5:
                return 0xB00;
            case 6:
                return 0xC00;
        }
    }

    getPixelsPerByte(screenMode)
    {
        switch(screenMode)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                return 8;
            case 4:
            case 5:
            case 6:
                return 16;
        }
    }

    plotPixel(imgData, idPtr, colour)
    {
        imgData.data[idPtr] = this.getRed(colour);
        imgData.data[idPtr+1] = this.getGreen(colour);
        imgData.data[idPtr+2] = this.getBlue(colour);
        imgData.data[idPtr+3] = this.getRed(colour) + this.getGreen(colour) + this.getBlue(colour) > 0 ? 255 : 0; // transparency

        return idPtr+4;
    }

    graphicsModeRedraw(ctx, imgData, processor)
    {   
        // Screen refresh initialisation
        var fastBlink = (processor.video.regs[10] & 32) == 0;
        if (++this.flashTime >= (fastBlink ? 12 : 32)) this.flashTime = 0;  // 3:1 flash ratio.
        this.flashOn = this.flashTime < (fastBlink ? 6 : 12);
        
        let screenMode = this.getScreenMode(processor);
        let screenStart = this.getScreenStart(screenMode);
        let screenSize = 0x8000 - screenStart;
        var offset = (((processor.video.regs[12] * 256) + processor.video.regs[13]) - this.getScreenOffset(screenMode)) * 8;
        var cursorPos = ((processor.video.regs[14] * 256) + processor.video.regs[15]) - this.getScreenOffset(screenMode) - (offset / 8);
        let idPtr = 0;
        let lineCount = 0;
      
        for (let memLoc = 0; memLoc < screenSize - (screenMode === 6 ? 0xc0 : 0) - (screenMode === 3 ? 0x180 : 0); memLoc++) {
            let byte = processor.readmem(screenStart + offset + memLoc);

            if(screenMode === 0) {
                for (let c=7; c>=0; c--) {
                    let colour = processor.video.actualPal[byte & (1<<c) ? 8 : 0];

                    idPtr = this.plotPixel(imgData, idPtr, colour);
                }
            }

            if(screenMode === 1) {
                for (let c=3; c>=0; c--) {
                    let colour = processor.video.actualPal[this.actual2bitColour[((byte & (1<<c)) ? 1 : 0) + ((byte & (1<<(c+4))) ? 2 : 0)]];
                    
                    // Two pixel width
                    idPtr = this.plotPixel(imgData, idPtr, colour);
                    idPtr = this.plotPixel(imgData, idPtr, colour);
                }
            }

            if(screenMode === 2) {
                for (let c=1; c>=0; c--) {
                    let colour = processor.video.actualPal[((byte & (1<<c)) !==0 ? 1 : 0) + ((byte & (1<<(c+2))) !==0 ? 2 : 0)
                            + ((byte & (1<<(c+4))) !==0 ? 4 : 0) + ((byte & (1<<(c+6))) !==0 ? 8 : 0)];
                  
                    // Four pixel width per bit
                    for (let i=0; i<4; i++) {
                        idPtr = this.plotPixel(imgData, idPtr, colour);
                    }
                }
            }

            if(screenMode === 3) {
                for (let c=7; c>=0; c--) {
                    let colour = processor.video.actualPal[byte & (1<<c) ? 8 : 0];

                    idPtr = this.plotPixel(imgData, idPtr, colour);
                }
            }

            if(screenMode === 4) {
                for (let c=7; c>=0; c--) {
                    let colour = processor.video.actualPal[byte & (1<<c) ? 8 : 0];
                   
                    // Two pixel width per bit
                    idPtr = this.plotPixel(imgData, idPtr, colour);
                    idPtr = this.plotPixel(imgData, idPtr, colour);
                }
            }    

            if(screenMode === 5) {
                for (let c=3; c>=0; c--) {
                    let colour = processor.video.actualPal[this.actual2bitColour[((byte & (1<<c)) !==0 ? 1 : 0) + ((byte & (1<<(c+4))) !==0 ? 2 : 0)]];
                    
                    // Four pixel width per bit
                    for (let i=0; i<4; i++) {
                        idPtr = this.plotPixel(imgData, idPtr, colour);
                    }
                }
            }

            if(screenMode === 6) {
                for (let c=7; c>=0; c--) {
                    let colour = processor.video.actualPal[byte & (1<<c) ? 8 : 0];
                   
                    // Two pixel width per bit
                    idPtr = this.plotPixel(imgData, idPtr, colour);
                    idPtr = this.plotPixel(imgData, idPtr, colour);
                }
            }

            // Move to start of next line for the current character
            idPtr -= this.getPixelsPerByte(screenMode) * 4;
            idPtr += 640 * 4;
            lineCount++;

            if(lineCount % 8 === 0) {
                // Advance one character, move pointer up and to the right
                idPtr -= (640 * 8) * 4;
                idPtr += (this.getPixelsPerByte(screenMode) * 4);
            }

            // Move to start of next character row
            if(lineCount % (8 * (640 / this.getPixelsPerByte(screenMode))) === 0) {
                idPtr += (639 * 8) * 4;
                idPtr -= 632 * 4;

                // Mode 3+6 skip 2 rows (characters are 8x10)
                if(screenMode === 3 || screenMode === 6)
                {
                    idPtr += (640 * 2 * 4);
                }
            }

            if((screenStart + offset + memLoc) === 0x7fff) {
                offset = offset - screenSize;
            }  
        }

        // Is the cursor flashing? (bit 6 of 6845 register 10)
        if((processor.video.regs[10] & 64) && processor.video.regs[11] && this.flashOn)
        {
            let cursorY = Math.floor(cursorPos / (640 / this.getPixelsPerByte(screenMode)));
            let cursorX = cursorPos - (cursorY * (640 / this.getPixelsPerByte(screenMode)));

            idPtr = (cursorY * 4 * (screenMode === 6 ? 10 : (screenMode === 3 ? 10 : 8)) * 8 * 80) + (cursorX * 4 * this.getPixelsPerByte(screenMode)); // Move to start of cursor char
            idPtr += (4 * 80 * 8 * 7); // Move down to start cursor line
            for(let i=0; i<8; i++)
            {
                let cursorWidth = (screenMode === 6 ? 2 : (screenMode === 5 ? 4 : (screenMode === 2 ? 4 : 
                    (screenMode === 3 ? 1 : (screenMode === 1 ? 2 : (screenMode === 4 ? 2 : 1))))));

                for(let j=0; j<cursorWidth; j++)
                {
                    imgData.data[idPtr] = 255;
                    imgData.data[idPtr+1] = 255;
                    imgData.data[idPtr+2] = 255;
                    imgData.data[idPtr+3] = 255;
                
                    idPtr+=4;
                }
            }
        }

        // Write screen to the canvas
        ctx.putImageData(imgData, 0, 0);
    }

    // Teletext redraw routine
    teletextRedraw(ctx, imgData, processor)
    {
        // Screen refresh initialisation
        var offset = ((processor.video.regs[12] * 256) + processor.video.regs[13]) - 0x2800;
        var cursorPos = ((processor.video.regs[14] * 256) + processor.video.regs[15]) - 0x2800 - offset;
        var cursorType = processor.video.regs[10];
        var fastBlink = (cursorType & 32) == 0;
        if (++this.flashTime >= (fastBlink ? 12 : 32)) this.flashTime = 0;  // 3:1 flash ratio.
        this.flashOn = this.flashTime < (fastBlink ? 6 : 12);
        let charPos = 0;

        // Read screen memory
        offset = 0x7c00 + offset;
        let pageBuffer = new Uint8Array(40 * 25);
        for(var i=0; i<40*25; i++) {
            if((offset + i) > 0x7fff) {
                offset = 0x7c00 - i;
            }
            pageBuffer[i] = processor.readmem((offset + i));
        }
       
        this.dbl = this.oldDbl = this.secondHalfOfDouble = this.wasDbl = false;

        for(var yCol = 0; yCol < 25; yCol++)
        {
            // Initialise row
            this.col = 7;
            this.bg = 0;
            this.holdChar = false;
            this.heldChar = 0x20;
            this.nextGlyphs = this.charSmoothed;
            this.heldGlyphs = this.charSmoothed;
            this.sep = false;
            this.gfx = false;
            this.dbl = false;
            this.flash = false;
            this.secondHalfOfDouble = this.secondHalfOfDouble ? false : this.wasDbl;
            this.wasDbl = false;
            
            for(var xCol = 0; xCol < 40; xCol++)
            {      
                this.oldDbl = this.dbl;
                this.prevCol = this.col;
                this.curGlyphs = this.nextGlyphs;
                this.prevFlash = this.flash;
                
                var idPtr = (xCol * 4 * 12) + (yCol * 40 * 4 * 12 * 20);
                var data = pageBuffer[charPos++];
                
                if(data >= 128)
                {
                    data = data - 128;
                }

                if (data < 0x20) {
                    data = this.handleControlCode(data);
                } else if (this.gfx) {
                    this.heldChar = data;
                    this.heldGlyphs = this.curGlyphs;
                } 

                // Displayable character, map to character definitions
                var charDef = (this.prevFlash && this.flashOn) || (this.secondHalfOfDouble && !this.dbl) ? 0 : data - 32;
                for(var yPos = 0; yPos < 20; yPos++)
                {
                    var actualY = this.dbl ? Math.floor(yPos / 2) + (this.secondHalfOfDouble ? 10 : 0) : yPos;
                    for(var xPos = 0; xPos < 12; xPos++)
                    {
                        let setPixel = this.curGlyphs[(charDef * 240) + (12 * actualY) + xPos] == 1;
                        imgData.data[idPtr] = setPixel ? this.getRGB_Red(this.prevCol) : this.getRGB_Red(this.bg);
                        imgData.data[idPtr+1] = setPixel ? this.getRGB_Green(this.prevCol) : this.getRGB_Green(this.bg);
                        imgData.data[idPtr+2] = setPixel ? this.getRGB_Blue(this.prevCol) : this.getRGB_Blue(this.bg);
                        imgData.data[idPtr+3] = (setPixel & this.col != 0) || (!setPixel && this.bg != 0) ? 255 : 0;
                        idPtr = idPtr + 4;
                    }
                    
                    idPtr = idPtr + (4 * 39 * 12);      
                }
                
                if (this.holdOff) {
                    this.holdChar = false;
                    this.heldChar = 32;
                }
            }
        }

        // Is the cursor flashing? (bit 6 of 6845 register 10)
        if((cursorType & 64) && this.flashOn)
        {
            let cursorY = Math.floor(cursorPos / 40);
            let cursorX = cursorPos - (cursorY * 40);

            idPtr = (cursorY * 4 * 12 * 20 * 40) + (cursorX * 4 * 12); // Move to start of cursor char
            idPtr += (4 * 40 * 12 * 18) + 4; // Move down to start cursor line
            for(let i=0; i<22; i++)
            {
                imgData.data[idPtr] = 255;
                imgData.data[idPtr+1] = 255;
                imgData.data[idPtr+2] = 255;
                imgData.data[idPtr+3] = 255;
                
                if(i == 10)
                {
                    idPtr = idPtr + (4 * 39 * 12) + 8; // Move down one row
                }
                else
                { 
                    idPtr+=4;
                }
            }
        }

        // Write screen to the canvas
        ctx.putImageData(imgData, 0, 0);
    }

    setNextChars()
    {
        if (this.gfx) {
            if (this.sep) {
                this.nextGlyphs = this.charSeparated;
            } else {
                this.nextGlyphs = this.charGraphics;
            }
        } else {
            this.nextGlyphs = this.charSmoothed;
        }
    }

    handleControlCode(data)
    {
        this.holdOff = false;
        switch (data) 
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                this.gfx = false;
                this.col = data;
                this.setNextChars();
                break;
            case 8:
                this.flash = true;
                break;
            case 9:
                this.flash = false;
                break;
            case 12:
            case 13:
                this.dbl = !!(data & 1);
                if (this.dbl) this.wasDbl = true;
                break;
            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
                this.gfx = true;
                this.col = data & 7;
                this.setNextChars();
                break;
            case 24:
                this.col = this.prevCol = this.bg;
                break;
            case 25:
                this.sep = false;
                this.setNextChars();
                break;
            case 26:
                this.sep = true;
                this.setNextChars();
                break;
            case 28:
                this.bg = 0;
                break;
            case 29:
                this.bg = this.col;
                break;
            case 30:
                this.holdChar = true;
                break;
            case 31:
                this.holdOff = true;
                break;
        }

        if (this.holdChar && this.dbl === this.oldDbl) {
            data = this.heldChar;
            if (data >= 0x40 && data < 0x60) data = 0x20;
            this.curGlyphs = this.heldGlyphs;
        } else {
            this.heldChar = 0x20;
            data = 0x20;
        }

        return data;
    }
}