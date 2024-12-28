"use strict";

import * as utils from "./utils.js";

// Code ported from Beebem (C to .js) by Jason Robson
const TELETEXT_IRQ = 5;
const TELETEXT_UPDATE_FREQ = 50000;

/*

Offset  Description                 Access  
+00     Status register             R/W
+01     Row register
+02     Data register
+03     Clear status register

Status register:
  Read
   Bits     Function
   0-3      Link settings
   4        FSYN (Latches high on Field sync)
   5        DEW (Data entry window)
   6        DOR (Latches INT on end of DEW)
   7        INT (latches high on end of DEW)
  
  Write
   Bits     Function
   0-1      Channel select
   2        Teletext Enable
   3        Enable Interrupts
   4        Enable AFC (and mystery links A)
   5        Mystery links B

*/

export class TeletextAdaptor {
    constructor(cpu) {
        this.cpu = cpu;
        this.teletextStatus = 0x0f; /* low nibble comes from LK4-7 and mystery links which are left floating */
        this.teletextInts = false;
        this.teletextEnable = false;
        this.channel = 0;
        this.currentFrame = 0;
        this.totalFrames = 0;
        this.rowPtr = 0x00;
        this.colPtr = 0x00;
        this.frameBuffer = new Array(16).fill(0).map(() => new Array(64).fill(0));
        this.streamData = null;
        this.pollCount = 0;
        this.curPos = 0;
        this.streamLength = 0;
    }

    reset(hard) {
        if (hard) {
            console.log("Teletext adaptor: initialisation");
            this.loadChannelStream(this.channel);
        }
    }

    loadChannelStream(channel) {
        console.log("Teletext adaptor: switching to channel " + channel);
        const teletextRef = this;
        utils.loadData("teletext/txt" + channel + ".dat").then(function (data) {
            teletextRef.streamData = data;
            teletextRef.streamLength = data.length;
            teletextRef.curPos = 42*7500;
        });
    }

    read(addr) {
        let data = 0x00;

        switch (addr) {
            case 0x00: // Status Register
                data = this.teletextStatus;
                break;
            case 0x01: // Row Register
                break;
            case 0x02: // Data Register
                data = this.frameBuffer[this.rowPtr][this.colPtr++];
                break;
            case 0x03:
                this.teletextStatus &= ~0xd0; // Clear INT, DOR, and FSYN latches
                this.cpu.interrupt &= ~(1 << TELETEXT_IRQ);
                break;
        }

        return data;
    }

    write(addr, value) {
        switch (addr) {
            case 0x00:
                // Status register
                this.teletextInts = (value & 0x08) === 0x08;
                if (this.teletextInts && this.teletextStatus & 0x80) {
                    this.cpu.interrupt |= 1 << TELETEXT_IRQ; // Interrupt if INT and interrupts enabled
                } else {
                    this.cpu.interrupt &= ~(1 << TELETEXT_IRQ); // Clear interrupt
                }
                this.teletextEnable = (value & 0x04) === 0x04;
                if ((value & 0x03) !== this.channel && this.teletextEnable) {
                    this.channel = value & 0x03;
                    this.loadChannelStream(this.channel);
                }
                break;

            case 0x01:
                this.rowPtr = value;
                this.colPtr = 0x00;
                break;

            case 0x02:
                this.frameBuffer[this.rowPtr][this.colPtr++] = value & 0xff;
                break;

            case 0x03:
                this.teletextStatus &= ~0xd0; // Clear INT, DOR, and FSYN latches
                this.cpu.interrupt &= ~(1 << TELETEXT_IRQ); // Clear interrupt
                break;
        }
    }

    // Attempt to emulate the TV broadcast
    polltime(cycles) {
        this.pollCount += cycles;
        if (this.pollCount > TELETEXT_UPDATE_FREQ) {
            this.pollCount = 0;
            // Don't flood the processor with teletext interrupts during a reset
            if (this.cpu.resetLine) {
                this.update();
            } else {
                // Grace period before we start up again
                this.pollCount = -TELETEXT_UPDATE_FREQ * 10;
            }
        }
    }


    deham(char)
    {
        let i = parseInt(char);

        if (i >= 0 && i <= 9)
        {
            return i+17;
        }

        if (i >= 10 && i <= 19)
        {
            return i+23;
        }

        if (i >= 20 && i <= 29)
        {
            return i+29;
        }

        if (i >= 30 && i <= 39)
        {
            return i+35;
        }
        
        if (i >= 40 && i <= 49)
        {
            return i+41;
        }
                    
        if (i >= 50 && i <= 59)
        {
            return i+47;
        }
        
        return 0;
    }

    update() {
        if(this.streamLength > 0)
        {
            if (this.curPos >= this.streamLength) {
                this.curPos = 0;
            }

            const offset = this.curPos;//this.currentFrame * TELETEXT_FRAME_SIZE + 2 * 42;

            this.teletextStatus &= 0x0f;
            this.teletextStatus |= 0xd0; // data ready so latch INT, DOR, and FSYN

            if (this.teletextEnable) {
                // Copy current stream position into the frame buffer
                
                for (let i = 0; i < 4; ++i) {
                    if (this.streamData[offset + i * 42] !== 0) {
                        this.frameBuffer[i][0] = 0x67;
                        for (let j = 0; j < 42; j++) {
                            this.frameBuffer[i][j+1] = this.streamData[offset + (i * 42) + j];
                        }

                        if((this.frameBuffer[i][1] === 0x15) && (this.frameBuffer[i][2] === 0xEA))      // Signature of the BSDP (magazine 8, packet 30)
                        {
                            let timeNow = new Date(Date.now());
                            this.frameBuffer[i][16] = this.deham(timeNow.getHours());        // Hours
                            this.frameBuffer[i][17] = this.deham(timeNow.getMinutes());      // Minutes
                            this.frameBuffer[i][18] = this.deham(timeNow.getSeconds());      // Seconds
                        
                            // Date
                            var today = new Date(); //set any date
                            var julian = Math.floor((today / 86400000) - (today.getTimezoneOffset() / 1440) + 2440587.5 - 2400000.5);
                            
                            // Add one to each digit (quirk of broadcast to avoid runs of all 0's or 1's)
                            var julian2 = "0";
                            for(let i=0; i<julian.toString().length; i++) {
                                julian2 += (parseInt(julian.toString()[i]) + 1).toString()[0];
                            }
                            
                            // Convert BCD
                            this.frameBuffer[i][13] = Number("0x" + julian2.substring(0, 2));
                            this.frameBuffer[i][14] = Number("0x" + julian2.substring(2, 4));
                            this.frameBuffer[i][15] = Number("0x" + julian2.substring(4, 6));
                        }

                    } else {
                        this.frameBuffer[i][0] = 0x00;
                    }

                }
            }
        
            this.currentFrame++;
            this.curPos = this.curPos + (42 * 4);

            this.rowPtr = 0x00;
            this.colPtr = 0x00;
        }

        if (this.teletextInts) {
            this.cpu.interrupt |= 1 << TELETEXT_IRQ;
        }
    }
}
