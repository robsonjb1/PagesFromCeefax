"use strict";
import * as utils from "./utils.js";

const TELETEXT_IRQ = 5;
const TELETEXT_UPDATE_FREQ = 50000;

// Hammed two-byte codes referencing a page number starting from 100
const pageRefs = [21,21,2,21,73,21,94,21,100,21,115,21,56,21,47,21,208,21,199,21,21,2,2,2,73,2,94,2,100,2,115,2,56,2,
    47,2,208,2,199,2,21,73,2,73,73,73,94,73,100,73,115,73,56,73,47,73,208,73,199,73,21,94,2,94,73,94,94,94,100,94,115,94,56,94,
    47,94,208,94,199,94,21,100,2,100,73,100,94,100,100,100,115,100,56,100,47,100,208,100,199,100,21,115,2,115,73,115,94,115,100,
    115,115,115,56,115,47,115,208,115,199,115,21,56];

// Hammed two-byte codes referencing a packet number starting from packet 0
const packetRefs = [199,21,2,2,199,2,2,73,199,73,2,94,199,94,2,100,199,100,2,115,199,115,2,56,
    199,56,2,47,199,47,2,208,199,208,2,199,199,199,2,140,199,140,2,155,199,155,2,161];

// Example packet 0 header row (which will be updated later)
const zeroPacket = [0x67, 2, 21, 21, 21, 21, 21, 21, 21, 21, 21, 67, 69, 69, 70, 193, 88, 32, 49, 32, 49, 181, 50, 32]
const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

const pageDuration = 27;

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
        this.carousel = null;
        this.currentPFCpage = 0;
        this.frameSequence = 0;
        this.usingUserStream = false;
    }

    reset(hard) {
        if (hard) {
            console.log("Teletext adaptor: initialisation");
        }
    }

    loadUserChannelStream(filename, data) {
        console.log("Teletext adaptor: loading channel stream " + filename + ", size " + data.byteLength);
      
        this.streamData = new Uint8Array(data);
        this.streamLength = data.byteLength;
        this.curPos = 0;
        this.linesPerFrame = filename.toUpperCase().includes(".F4.") ? 4 : filename.toUpperCase().includes(".F6.") ? 6 : filename.toUpperCase().includes(".F8.") ? 8 : 16;
        this.usingUserStream = true;
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

    polltime(cycles) {
        this.pollCount += cycles;
        if (this.pollCount > TELETEXT_UPDATE_FREQ) {
            this.pollCount -= TELETEXT_UPDATE_FREQ;
            if (this.cpu.resetLine && this.cpu.p.i && this.teletextInts) {
                this.update();
            }

            if (!this.cpu.resetLine) {
                // Grace period before we start up again
                this.pollCount = -TELETEXT_UPDATE_FREQ * 10;
                this.teletextInts = false;
                this.teletextEnable = false;
            }
        }

        // Do we need to refresh the PFC magazine ? (takes place every 30 minutes)
        const now = new Date();
        if (now - this.carouselRequestTime > (30 * 60 * 1000)) {
            this.carouselRequestTime = now;
            
            // Refresh magazine
            this.getNewCarousel();
        }
    }

    getNewCarousel() {
        // Mark the time we first requested the magazine
        this.carouselRequestTime = new Date();
        
        // Request the carousel
        let self = this;
        $.ajax ({
            dataType: 'json',
            url: "../carousel",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                self.carousel = data.carousel;
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

    markUnusedFrameRows(start, finish)
    {
        for (let i=start; i < finish; i++) {
            this.frameBuffer[i][0] = 0;
        }
    }

    update() {
        if (this.curPos >= this.streamLength) {
            this.curPos = 0;
        }

        this.teletextStatus &= 0x0f;
        this.teletextStatus |= 0xd0; // data ready so latch INT, DOR, and FSYN
        
        if(this.usingUserStream)
        {
            // Copy current stream position into the frame buffer
            for (let i = 0; i < this.linesPerFrame; i++) {
                this.frameBuffer[i][0] = 0x67;
                for (let j = 0; j < 42; j++) {
                    this.frameBuffer[i][j+1] = this.streamData[this.curPos + (i * 42) + j];
                }
            }

            if(this.linesPerFrame < 16) {
                this.markUnusedFrameRows(this.linesPerFrame, 16);
            }

            this.currentFrame++;
            this.curPos = this.curPos + (42 * this.linesPerFrame);
            this.rowPtr = 0x00;
            this.colPtr = 0x00;
        }
        else
        {
            if (this.carousel != null) {
                const now = new Date();
                const carouselPFCpage = (Math.floor(((now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) / pageDuration)) % this.carousel.totalPages;

                // Page 100 will show a cycling PFC page from the rest of the carousel, so hence an effective and current PFC page
                var effectivePFCpage = (this.currentPFCpage > 0) ? this.currentPFCpage - 1 : carouselPFCpage;

                if(this.frameSequence === 0) {
                    // Write header packet for the current page
                    for(let i=0; i<zeroPacket.length; i++) {
                        this.frameBuffer[0][i] = zeroPacket[i];
                    }
                                
                    // Insert page number into header packet
                    this.frameBuffer[0][3] = pageRefs[this.currentPFCpage * 2];             // Encoded page number byte 1
                    this.frameBuffer[0][4] = pageRefs[(this.currentPFCpage * 2) + 1];       // Encoded page number byte 2
                    this.frameBuffer[0][21] = 48 + Math.floor(this.currentPFCpage / 10);    // Displayable tens
                    this.frameBuffer[0][22] = 48 + this.currentPFCpage % 10;                // Displayable units
                    
                    // Update date and time
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
                    this.frameBuffer[1][16] = this.deham(timeNow.getHours());    
                    this.frameBuffer[1][17] = this.deham(timeNow.getMinutes());  
                    this.frameBuffer[1][18] = this.deham(timeNow.getSeconds());  
                
                    // Date
                    var today = new Date(); 
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

                    this.markUnusedFrameRows(2, 16);
                }

                // Write page content
                if(this.frameSequence === 1)
                {
                    // Output page rows 1 to 16
                    for (let i=0; i<16; i++) {
                        this.frameBuffer[i][0] = 0x67;
                        this.frameBuffer[i][1] = packetRefs[i*2];
                        this.frameBuffer[i][2] = packetRefs[(i*2)+1];
                        for (let j=0; j<40; j++) {
                            this.frameBuffer[i][j+3] = this.carousel.content[effectivePFCpage].data[40 + ((i*40)+j)]; // Skip first row as it is blank
                        }
                    }
                }
                
                if(this.frameSequence === 2)
                {
                    // Output remaining page rows 17 to 24
                    for (let i=16; i<25; i++) {
                        this.frameBuffer[i-16][0] = 0x67;
                        this.frameBuffer[i-16][1] = packetRefs[i*2];
                        this.frameBuffer[i-16][2] = packetRefs[(i*2)+1];
                        for (let j=0; j<40; j++) {
                            this.frameBuffer[i-16][j+3] = this.carousel.content[effectivePFCpage].data[40 + ((i*40)+j)];
                        }
                    }
                    this.markUnusedFrameRows(8, 16);
                }

                if(this.frameSequence === 3) { // Used to slow down the counter, frame 3 is intentionally blank
                    this.markUnusedFrameRows(0, 16);
                }

                // Update for the next frame
                this.frameSequence++;
                if(this.frameSequence === 4) {
                    this.frameSequence = 0;
                    this.currentPFCpage++;
                    if(this.currentPFCpage > this.carousel.totalPages)
                    {
                        this.currentPFCpage = 0;
                    }
                }
            }
        }
        
        if (this.teletextInts) {
            this.cpu.interrupt |= 1 << TELETEXT_IRQ;
        }
    }
}
