
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
                // Write 2x2 block
                smoothedData[destPos] = smoothedData[destPos+1] = smoothedData[destPos+12] = smoothedData[destPos+13] = sourcePixel;
            }
        }

        // Smooth the character
        for(var row=0; row<19; row++)
        {
            for(var pixel=0; pixel<11; pixel++)
            {
                var sourcePos = (charNo * 240) + (row * 12) + pixel;
                if((smoothedData[sourcePos] == 1                 
                    && smoothedData[sourcePos+1] == 0           // 1 0
                    && smoothedData[sourcePos+12] == 0           // 0 1
                    && smoothedData[sourcePos+13] == 1          
                ) || (smoothedData[sourcePos] == 0                 
                    && smoothedData[sourcePos+1] == 1           // 0 1
                    && smoothedData[sourcePos+12] == 1           // 1 0
                    && smoothedData[sourcePos+13] == 0          
                )) {
                    // Write 2x2 block
                    smoothedData[sourcePos] = smoothedData[sourcePos+1] = smoothedData[sourcePos+12] = smoothedData[sourcePos+13] = 1;
                }
            }
        }
    }

    return smoothedData;


}