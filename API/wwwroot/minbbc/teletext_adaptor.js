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
        this.rowPtr = 0x00;
        this.colPtr = 0x00;
        this.frameBuffer = new Array(16).fill(0).map(() => new Array(64).fill(0));
        this.streamData = null;
        this.pollCount = 0;
        this.curPos = 0;
        this.streamLength = 0;
        this.linesPerFrame = 16;
        this.carouselRequestTime = 0;
        this.carousel = 0;
    }

    reset(hard) {
        if (hard) {
            console.log("Teletext adaptor: initialisation");
            this.loadDefaultChannelStream();
        }
    }

    loadUserChannelStream(filename, data) {
        console.log("Teletext adaptor: loading channel stream " + filename + ", size " + data.byteLength);
      
        this.streamData = new Uint8Array(data);
        this.streamLength = data.byteLength;
        this.curPos = 0;
        this.linesPerFrame = filename.toUpperCase().includes(".F4.") ? 4 : filename.toUpperCase().includes(".F6.") ? 6 : filename.toUpperCase().includes(".F8.") ? 8 : 16;
    }

    loadDefaultChannelStream() {
        const filename="BBC1 2006-01-22.t42";    // The default feed
        console.log("Teletext adaptor: loading default channel stream " + filename);

        const teletextRef = this;
        utils.loadData("teletext/" + filename).then(function (data) {
            teletextRef.streamData = data;
            teletextRef.streamLength = data.length;
            teletextRef.curPos = 42 * 310;
            teletextRef.linesPerFrame = 16;
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
                this.pollCount = -TELETEXT_UPDATE_FREQ * 20;
            }
        }

        const now = new Date();
      
        // Do we need to refresh the magazine ? (takes place every 30 minutes)
        if (now - this.carouselRequestTime > (30 * 1000)) {
            this.carouselRequestTime = now;
            
            // Refresh magazine
            this.getNewCarousel();
        }
    }

    getNewCarousel() {
        // Mark the time we first requested the magazine
        this.carouselRequestTime = new Date();
        
        // Request the carousel
        $.ajax ({
            dataType: 'json',
            url: "../carousel",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                this.carousel = data.carousel;
                console.log("Teletext adaptor: New carousel received");
            }
        });
    }

    deham(char)
    {
        let i = parseInt(char);

        if (i >= 0 && i <= 9) { return i+17; }
        if (i >= 10 && i <= 19) { return i+23; }
        if (i >= 20 && i <= 29) { return i+29; }
        if (i >= 30 && i <= 39) { return i+35; }
        if (i >= 40 && i <= 49) { return i+41; }
        if (i >= 50 && i <= 59) { return i+47; }
        
        return 0;
    }

    update() {
        if(this.streamLength > 0) {
            if (this.curPos >= this.streamLength) {
                this.curPos = 0;
            }

            this.teletextStatus &= 0x0f;
            this.teletextStatus |= 0xd0; // data ready so latch INT, DOR, and FSYN

            if (this.teletextEnable) {
                if(this.channel == 0)
                {
                    // Copy current stream position into the frame buffer
                    for (let i = 0; i < this.linesPerFrame; i++) {
                        this.frameBuffer[i][0] = 0x67;
                        for (let j = 0; j < 42; j++) {
                            this.frameBuffer[i][j+1] = this.streamData[this.curPos + (i * 42) + j];
                        }
                    }

                    if(this.linesPerFrame < 16) {
                        // Mark unused rows
                        for (let i=this.linesPerFrame; i < 16; i++) {
                            this.frameBuffer[i][0] = 0;
                        }
                    }

                    this.currentFrame++;
                    this.curPos = this.curPos + (42 * this.linesPerFrame);
                    this.rowPtr = 0x00;
                    this.colPtr = 0x00;
                }
                else
                {
                    // Write header packet for p100
                    const zeroPacket = [0x67, 2, 21, 73, 115, 21, 21, 21, 21, 21, 21, 67, 69, 69, 70, 193, 88, 32, 49, 32, 49, 181, 50, 32]
                    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                    const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
                    const now = new Date();

                    for(let i=0; i<zeroPacket.length; i++) {
                        this.frameBuffer[0][i] = zeroPacket[i];
                    }
                                    
                    const dateTime = days[now.getDay()] + ' ' + ('0' + now.getDate()).slice(-2) + ' ' + months[now.getMonth()] + 
                        String.fromCharCode(3) +
                        ('0' + now.getHours()).slice(-2) + ':' + ('0' + now.getMinutes()).slice(-2) + '/' + ('0' + now.getSeconds()).slice(-2);

                    for (let i=0; i<dateTime.length; i++) {
                        this.frameBuffer[0][i+24] = dateTime.charCodeAt(i);
                    }
                    
                    // Write BDSP
                    const BDSPPacket = [0x67, 21, 234, 21, 21, 21, 234, 234, 234, 94, 95, 246, 129, 22, 72, 104, 25, 37, 57,
                        21, 21, 21, 21, 194, 194, 67, 32, 79, 206, 69, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32];

                    for(let j=0; j<BDSPPacket.length; j++) {
                        this.frameBuffer[1][j] = BDSPPacket[j];
                    }

                    let timeNow = new Date(Date.now());
                    this.frameBuffer[1][16] = this.deham(timeNow.getHours());        // Hours
                    this.frameBuffer[1][17] = this.deham(timeNow.getMinutes());      // Minutes
                    this.frameBuffer[1][18] = this.deham(timeNow.getSeconds());      // Seconds
                
                    // Date
                    var today = new Date(); //set any date
                    var julian = Math.floor((today / 86400000) - (today.getTimezoneOffset() / 1440) + 2440587.5 - 2400000.5);
                    
                    // Add one to each digit (quirk of broadcast to avoid runs of all 0's or 1's)
                    var julian2 = "0";
                    for(let i=0; i<julian.toString().length; i++) {
                        julian2 += (parseInt(julian.toString()[i]) + 1).toString()[0];
                    }
                    
                    // Convert BCD
                    this.frameBuffer[1][13] = Number("0x" + julian2.substring(0, 2));
                    this.frameBuffer[1][14] = Number("0x" + julian2.substring(2, 4));
                    this.frameBuffer[1][15] = Number("0x" + julian2.substring(4, 6));
                    
                    // Mark unused rows to 16
                    for (let i=2; i < 16; i++) {
                        this.frameBuffer[i][0] = 0;
                    }
                }
            }
        }

        if (this.teletextInts) {
            this.cpu.interrupt |= 1 << TELETEXT_IRQ;
        }
    }
}
