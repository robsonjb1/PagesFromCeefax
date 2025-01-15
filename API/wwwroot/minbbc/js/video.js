"use strict";
import * as utils from "./utils.js";

////////////////////
// ULA interface
class Ula {
    constructor(video) {
        this.video = video;
    }
    write(addr, val) {
        addr |= 0;
        val |= 0;
        if (addr & 1) {
            const index = (val >>> 4) & 0xf;
            this.video.actualPal[index] = val & 0xf;
            let ulaCol = val & 7;
            if (!(val & 8 && this.video.ulactrl & 1)) ulaCol ^= 7;
            if (this.video.ulaPal[index] !== this.video.collook[ulaCol]) {
                this.video.ulaPal[index] = this.video.collook[ulaCol];
            }
        } else {
           
            this.video.ulactrl = val;
            this.video.pixelsPerChar = val & 0x10 ? 8 : 16;
            this.video.halfClock = !(val & 0x10);
            const newMode = (val >>> 2) & 3;
            if (newMode !== this.video.ulaMode) {
                this.video.ulaMode = newMode;
            }
            this.video.teletextMode = !!(val & 2);
        }
    }
}

////////////////////
// CRTC interface
class Crtc {
    constructor(video) {
        this.video = video;
        this.curReg = 0;
        this.crtcmask = new Uint8Array([
            0xff, 0xff, 0xff, 0xff, 0x7f, 0x1f, 0x7f, 0x7f, 0xf3, 0x1f, 0x7f, 0x1f, 0x3f, 0xff, 0x3f, 0xff, 0x3f, 0xff,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        ]);
    }
    read(addr) {
        if (!(addr & 1)) return 0;
        switch (this.curReg) {
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
                return this.video.regs[this.curReg];
        }
        return 0;
    }
    write(addr, val) {
        if (addr & 1) {
            this.video.regs[this.curReg] = val & this.crtcmask[this.curReg];
          
        } else this.curReg = val & 31;
    }
}

////////////////////
// The video class
export class Video {
    constructor(paint_ext_param) {
        this.fb32 = utils.makeFast32(1000);
        this.collook = utils.makeFast32(
            new Uint32Array([
                0xff000000, 0xff0000ff, 0xff00ff00, 0xff00ffff, 0xffff0000, 0xffff00ff, 0xffffff00, 0xffffffff,
            ])
        );
        this.screenAddrAdd = new Uint16Array([0x4000, 0x3000, 0x6000, 0x5800]);
        this.cursorTable = new Uint8Array([0x00, 0x00, 0x00, 0x80, 0x40, 0x20, 0x20]);
        this.cursorFlashMask = new Uint8Array([0x00, 0x00, 0x08, 0x10]);
        this.regs = new Uint8Array(32);
        this.bitmapX = 0;
        this.bitmapY = 0;
        this.oddClock = false;
        this.frameCount = 0;
        this.doEvenFrameLogic = false;
        this.isEvenRender = true;
        this.lastRenderWasEven = false;
        this.firstScanline = true;
        this.inHSync = false;
        this.inVSync = false;
        this.hadVSyncThisRow = false;
        this.checkVertAdjust = false;
        this.endOfMainLatched = false;
        this.endOfVertAdjustLatched = false;
        this.endOfFrameLatched = false;
        this.inVertAdjust = false;
        this.inDummyRaster = false;
        this.hpulseWidth = 0;
        this.vpulseWidth = 0;
        this.hpulseCounter = 0;
        this.vpulseCounter = 0;
        this.horizCounter = 0;
        this.vertCounter = 0;
        this.scanlineCounter = 0;
        this.vertAdjustCounter = 0;
        this.addr = 0;
        this.lineStartAddr = 0;
        this.nextLineStartAddr = 0;
        this.ulactrl = 0;
        this.pixelsPerChar = 8;
        this.halfClock = false;
        this.ulaMode = 0;
        this.teletextMode = true;
        this.displayEnableSkew = 0;
        this.ulaPal = utils.makeFast32(new Uint32Array(16));
        this.actualPal = new Uint8Array(16);
        this.cursorOn = false;
        this.cursorOff = false;
        this.cursorOnThisFrame = false;
        this.cursorDrawIndex = 0;
        this.cursorPos = 0;
        this.interlacedSyncAndVideo = false;
        this.doubledScanlines = true;
        this.frameSkipCount = 0;
        this.screenAdd = 0;
        this.paint_ext = paint_ext_param;
        this.clockCounter = 0;

        this.crtc = new Crtc(this);
        this.ula = new Ula(this);

        this.reset(null);
    }

    reset(cpu, via) {
        this.cpu = cpu;
        this.sysvia = via;
    }

    setScreenAdd(viaScreenAdd) {
        this.screenAdd = this.screenAddrAdd[viaScreenAdd];
    }


    ////////////////////
    // Main drawing routine
    polltime(clocks) {
        this.clockCounter += clocks;
        if(this.clockCounter > 50000) 
        {
            this.clockCounter -= 50000;
            this.sysvia.setVBlankInt(true);
            this.paint_ext();
            this.sysvia.setVBlankInt(false);
        }

    }
}
