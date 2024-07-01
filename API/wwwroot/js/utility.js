// Teletext rendering utilities

function getRGB_Red(col)
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

function getRGB_Green(col)
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

function getRGB_Blue(col)
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

function makeSmoothedChars(charData)
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

                solidBlock(smoothedData, destPos, 2, 2, sourcePixel, false);
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
                    solidBlock(smoothedData, sourcePos, 2, 2, 1, false);
                }
            }
        }
    }

    return smoothedData;
}

function solidBlock(graphicData, startPos, width, height, val, sep)
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

function makeGraphicChars(charData, sep)
{
    // Construct the graphic data using the smoothed chars as a template (as we need to preserve the CAPS alpha chars)
    var graphicData = new makeSmoothedChars(charData);

    for(var char=0; char<64; char++)
    {
        var startPos = (char * 12 * 20) + (char > 31 ? (32 * 12 * 20) : 0);             // Add offset to skip over the CAPS alpha chars

        solidBlock(graphicData, startPos, 6, 6, !!(char & 1), sep);                     // Top left
        solidBlock(graphicData, startPos + 6, 6, 6, !!(char & 2), sep);                 // Top right
        solidBlock(graphicData, startPos + (12 * 6), 6, 8, !!(char & 4), sep);          // Middle left, row is larger at 8 pixels
        solidBlock(graphicData, startPos + (12 * 6) + 6, 6, 8, !!(char & 8), sep);      // Middle right, row is larger at 8 pixels
        solidBlock(graphicData, startPos + (12 * 14), 6, 6, !!(char & 16), sep);        // Bottom left
        solidBlock(graphicData, startPos + (12 * 14) + 6, 6, 6, !!(char & 32), sep);    // Bottom right
    }

    return graphicData;
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
    const footer = " Music " + (musicOn ? "Off" : "On ") + "  Previous   Next    Twitter/X";
    
    for(var i=0; i<footer.length; i++) charData[960+i] = footer.charCodeAt(i);
    charData[960] = 1; // Red
    charData[970] = 2; // Green
    charData[982] = 3; // Yellow
    charData[989] = 6; // Cyan
}

function getCursorPosition(canvas, ev) {
    var rect = canvas.getBoundingClientRect()
    var x = ev.clientX - rect.left
    var y = ev.clientY - rect.top
    return {x, y};
}
