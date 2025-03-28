///////////////////////////////////////////////////////////////////////////////
/// @file ZXScreen.js
///
/// @brief Screen abstraction for the MinZX 48K Spectrum emulator
///
/// @author David Crespo Tascon
///
/// @copyright (c) David Crespo Tascon
///  This code is released under the MIT license,
///  a copy of which is available in the associated LICENSE file,
///  or at http://opensource.org/licenses/MIT
///////////////////////////////////////////////////////////////////////////////

"use strict";

// Documentation for spectrum screen:
// http://www.breakintoprogram.co.uk/computers/zx-spectrum/screen-memory-layout

class ZXScreen
{
    constructor(canvasIdForScreen)
    {
        // create canvas and context
        this.canvas = document.getElementById(canvasIdForScreen);
        this.ctx = this.canvas.getContext('2d');
        this.canvas.width = 256; 
        this.canvas.height = 192;

        // create image data for screen
        this.zxid = new ZXScreenAsImageData(this.ctx);
    }

    update(mem, flashstate, channelOverlay)
    {
        // generate image data from array and flash state
        this.zxid.putSpectrumImage(mem, flashstate, channelOverlay);
        this.ctx.putImageData(this.zxid.imgdata, 0, 0);
    }
}

// ZX Spectrum colors. Using 192 for non-bright value, 255 for bright value.
const zxcolors = [
    [  0,   0,   0, 0],
    [  0,   0, 192, 255],
    [192,   0,   0, 255],
    [192,   0, 192, 255],
    [  0, 192,   0, 255],
    [  0, 192, 192, 255],
    [192, 192,   0, 255],
    [192, 192, 192, 255],
    [  0,   0,   0, 0],
    [  0,   0, 255, 255],
    [255,   0,   0, 255],
    [255,   0, 255, 255],
    [  0, 255,   0, 255],
    [  0, 255, 255, 255],
    [255, 255,   0, 255],
    [255, 255, 255, 255]
];

class ZXScreenAsImageData
{
    constructor(ctx)
    {
        // add border to image data dimensions
        this.width = 256;
        this.height = 192;

        // create image data
        this.imgdata = ctx.createImageData(this.width, this.height);
    }

    // accessors fo actual dimensions
    getWidth () { return this.width;  }
    getHeight() { return this.height; }

    // color for bitmap bits with 0 (PAPER) value
    getAttrColorIndexForBit0(attr)
    {
        let bri = (attr & 0x40) != 0 ? 0x08 : 0x00;
        let rgb = (attr & 0x38) >> 3;
        return zxcolors[rgb | bri];
    }

    // color for bitmap bits with 0 (INK) value
    getAttrColorIndexForBit1(attr)
    {
        let bri = (attr & 0x40) != 0 ? 0x08 : 0x00;
        let rgb = (attr & 0x07);
        return zxcolors[rgb | bri];
    }

    putChar(linscr, mem, index, char)
    {
        const chars = 0x3d00 + (char * 8);      // Copy from Spectrum character ROM
        var startPos = index + (23 * 32 * 8);   // Start on row 23

        for (var i = 0; i < 8; i++)
        {
            linscr[startPos + (i * 32)] = mem[chars + i];
        }
        
        linscr[6144 + index + (23 * 32)] = 64 + 4;   // Attributes for row 23 (bright + green)
    }


    // Generate image data for spectrum screen
    // - zxscreen: spectrum screen data (6912 btes),
    // - flashinv: indicates if flash attribute is to be inverted now
    putSpectrumImage(mem, flashinv, channelOverlay)
    {
        // de-interlace zx-screen to a linear bitmap
        let linscr = this.zx_row_adjust(mem);

        // Display channel banner overlay
        if(channelOverlay)
        {
            let index = 0;
            for(let char of channelOverlay)
            {
                this.putChar(linscr, mem, index, ASCIItoCharIndex(char));
                index++;
            }
        }
        
        // source and destination indices
        let isrc = 0;
        let idst = 0;

        // shortcut for image data
        const data = this.imgdata.data;

        // calculate where to start for topleft pixel
        idst = 0;
       
        // traverse all 24 rows
        for (let row = 0; row < 24; row++)
        {
            // traverse 8 subrows in row
            for (let subrow = 0; subrow < 8; subrow++)
            {
                // index of attribute for first character in row
                let iatt = 6144 + 32 * row;

                // traverse 32 bitmap bytes in subrow
                for (let x = 0; x < 32; x++)
                {
                    const attr = linscr[iatt++];  // attribute
                    let   byte = linscr[isrc++];  // bitmap byte

                    // PAPER color (for bits with value 0)
                    let col0 = this.getAttrColorIndexForBit0(attr);

                    // PAPER color (for bits with value 0)
                    let col1 = this.getAttrColorIndexForBit1(attr);

                    // if attribute has FLASH, and we are in that part of cycle,
                    // invert (switch PAPER and INK colors)
                    if (flashinv && (attr & 0x80)) {
                        let aux = col0; col0 = col1; col1 = aux;
                    }

                    // traverse 8 bits in byte
                    for (let b = 0; b < 8; b++)
                    {
                        let bit = 0;
                        if ((byte & 0x80) != 0)
                            bit = 1;
                        byte <<= 1;

                        // put INK color for 1, PAPER color for 0
                        if (bit) {
                            data[idst++] = col1[0]; // r
                            data[idst++] = col1[1]; // g
                            data[idst++] = col1[2]; // b
                            data[idst++] = col1[3]; // a
                        }
                        else {
                            data[idst++] = col0[0]; // r
                            data[idst++] = col0[1]; // g
                            data[idst++] = col0[2]; // b
                            data[idst++] = col0[3]; // a
                        }
                    }
                }
            }
        }
    }

    // 'deinterlace' screen rows
    zx_row_adjust(src)
    {
        const off = 0x4000;

        // create array for deinterlaced screen
        let dst = new Uint8Array(6912);

        // traverse all 192 rows
        for (let row = 0; row < 192; row++)
        {
            // bit juggle for calculating spectrum row index
            let rzx = ((row & 0x38) >> 3) | ((row & 0x07) << 3) | (row & 0xC0);
            let isrc = row * 32;
            let idst = rzx * 32;
            // copy row
            for (let col = 0; col < 32; col++)
                dst[idst++] = src[off + isrc++]
        }

        // copy attributes
        for (let i = 0; i < 768; i++)
        {
            dst[6144+i] = src[off + 6144+i];
        }

        return dst;
    }
}
