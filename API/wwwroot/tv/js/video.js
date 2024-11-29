
export class video {
    
    constructor()
    {
        this.flashTimer = 0;
    }

    //Black = 0,
    //Blue = 1,
    //Red = 2,
    //Magenta = 3,
    //Green = 4,
    //Cyan = 5,
    //Yellow = 6,
    //White = 7

    getRGB_Red(col, intensity)
    {
        switch(col)
        {
            case 2:
            case 3:
            case 6:
            case 7:
                return intensity;
            default: 
                return 0;
        }
    }

    getRGB_Green(col, intensity)
    {
        switch(col)
        {
            case 4:
            case 5:
            case 6:
            case 7:
                return intensity;
            default: 
                return 0;
        }
    }

    getRGB_Blue(col, intensity)
    {
        switch(col)
        {
            case 1:
            case 3:
            case 5:
            case 7:
                return intensity;
            default: 
                return 0;
        }
    }

    readScreenMem(snafile, xCol, yCol)
    {

        var loc = 27;

        // 0 1 0 Y7 Y6 Y2 Y1 YO y5 y4 Y3 x4 x3 x2 x1 x0

        loc = loc + (((yCol >> 7) & 0x1) * 4096);
        loc = loc + (((yCol >> 6) & 0x1) * 2048);
        loc = loc + (((yCol >> 2) & 0x1) * 1024);
        loc = loc + (((yCol >> 1) & 0x1) * 512);
        loc = loc + (((yCol >> 0) & 0x1) * 256);
        loc = loc + (((yCol >> 5) & 0x1) * 128);
        loc = loc + (((yCol >> 4) & 0x1) * 64);
        loc = loc + (((yCol >> 3) & 0x1) * 32);
        
        // 0x41dd start



        loc = loc + Math.floor(xCol / 8);
  

        var bit = 7 - (xCol % 8);

        return snafile[loc] & (1 << bit) ? true : false; // Offset the SNA header
    }
    

    // Main redraw routine
    redraw(ctx, snafile, imgData)
    {
        var cPtr = 0;
        
        for(var yCol = 0; yCol < 192; yCol++)
        {
            for(var xCol = 0; xCol < 256; xCol++)
            {
                var pixel = this.readScreenMem(snafile, xCol, yCol);
                var attributePos = 6171 + (Math.floor(xCol / 8)) + (Math.floor(yCol / 8) * 32);
                var attribute = snafile[attributePos];

                var ink = attribute & 7; // bits 0 to 2
                var paper = (attribute & 56) >> 3; // bits 3 to 5
                var intensity = (attribute & 64) ? 255 : 192; // bit 6
                var flash = (attribute & 128) // bit 7

                if(flash && this.flashTimer > 15) { 
                    paper = attribute & 7; // bits 0 to 2
                    ink = (attribute & 56) >> 3; // bits 3 to 5
                }

                imgData.data[cPtr] = pixel ? this.getRGB_Red(ink, intensity) : this.getRGB_Red(paper, intensity);
                imgData.data[cPtr+1] = pixel ? this.getRGB_Green(ink, intensity) : this.getRGB_Green(paper, intensity);
                imgData.data[cPtr+2] = pixel ? this.getRGB_Blue(ink, intensity) : this.getRGB_Blue(paper, intensity);
                imgData.data[cPtr+3] = (pixel && ink > 0) || (!pixel && paper > 0) ? 255 : 0;

                cPtr+= 4;
            }
        }

        this.flashTimer++;

        if(this.flashTimer > 30)
        {
            this.flashTimer = 0;
        }

        // Write screen to the canvas
        ctx.putImageData(imgData, 0, 0);
    }
}