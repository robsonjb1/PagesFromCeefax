"use strict";
import * as utils from "./utils.js";
import * as opcodes from "./6502.opcodes.js";
import * as via from "./via.js";
import { Scheduler } from "./scheduler.js";
import { TeletextAdaptor } from "./teletext-adaptor.js";
import { WD1770 } from "./fdc.js";

const signExtend = utils.signExtend;

const beebSwram = [
    true,
    true,
    true,
    true, // Dunjunz variants. Exile (not picky).
    true,
    true,
    true,
    true, // Crazee Rider.
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
];

function _set(byte, mask, set) {
    return (byte & ~mask) | (set ? mask : 0);
}
class Flags {
    constructor() {
        this._byte = 0x30;
    }
    get c() {
        return !!(this._byte & 0x01);
    }
    set c(val) {
        this._byte = _set(this._byte, 0x01, val);
    }
    get z() {
        return !!(this._byte & 0x02);
    }
    set z(val) {
        this._byte = _set(this._byte, 0x02, val);
    }
    get i() {
        return !!(this._byte & 0x04);
    }
    set i(val) {
        this._byte = _set(this._byte, 0x04, val);
    }
    get d() {
        return !!(this._byte & 0x08);
    }
    set d(val) {
        this._byte = _set(this._byte, 0x08, val);
    }
    get v() {
        return !!(this._byte & 0x40);
    }
    set v(val) {
        this._byte = _set(this._byte, 0x40, val);
    }
    get n() {
        return !!(this._byte & 0x80);
    }
    set n(val) {
        this._byte = _set(this._byte, 0x80, val);
    }
    reset() {
        this._byte = 0x30;
    }
    debugString() {
        return (
            (this.n ? "N" : "n") +
            (this.v ? "V" : "v") +
            "xx" +
            (this.d ? "D" : "d") +
            (this.i ? "I" : "i") +
            (this.z ? "Z" : "z") +
            (this.c ? "C" : "c")
        );
    }
    asByte() {
        return this._byte | 0x30;
    }
    setFromByte(byte) {
        this._byte = byte | 0x30;
    }
}

class Base6502 {
    constructor() {
        this.a = this.x = this.y = this.s = 0;
        this.pc = 0;
        this.opcodes = opcodes.Cpu6502(this);
        this.disassembler = this.opcodes.disassembler;
        this.forceTracing = false;
        this.runner = this.opcodes.runInstruction;

        this.adc = function (addend) {
            if (!this.p.d) {
                this.adcNonBCD(addend);
            } else {
                this.adcBCD(addend);
            }
        };

        this.sbc = function (subend) {
            if (!this.p.d) {
                this.adcNonBCD(subend ^ 0xff);
            } else {
                this.sbcBCD(subend);
            }
        };
    }

    incpc() {
        this.pc = (this.pc + 1) & 0xffff;
    }

    getb() {
        const result = this.readmem(this.pc);
        this.incpc();
        return result | 0;
    }

    getw() {
        let result = this.readmem(this.pc) | 0;
        this.incpc();
        result |= (this.readmem(this.pc) | 0) << 8;
        this.incpc();
        return result | 0;
    }

    checkInt() {
        this.takeInt = !!(this.interrupt && !this.p.i);
        this.takeInt |= this.nmi;
    }

    setzn(v) {
        v &= 0xff;
        this.p.z = !v;
        this.p.n = !!(v & 0x80);
        return v | 0;
    }

    push(v) {
        this.writememZpStack(0x100 + this.s, v);
        this.s = (this.s - 1) & 0xff;
    }

    pull() {
        this.s = (this.s + 1) & 0xff;
        return this.readmemZpStack(0x100 + this.s);
    }

    NMI(nmi) {
        this.nmi = !!nmi;
    }

    brk(isIrq) {
        // Behavior here generally discovered via Visual 6502 analysis.
        // 6502 has a quirky BRK; it was sanitized in 65c12.
        // See also https://wiki.nesdev.com/w/index.php/CPU_interrupts
        let pushAddr = this.pc;
        if (!isIrq) pushAddr = (pushAddr + 1) & 0xffff;
        this.readmem(pushAddr);

        this.push(pushAddr >>> 8);
        this.push(pushAddr & 0xff);
        let pushFlags = this.p.asByte();
        if (isIrq) pushFlags &= ~0x10;
        this.push(pushFlags);

        // NMI status is determined part way through the BRK / IRQ
        // sequence, and yes, on 6502, an NMI can redirect the vector
        // for a half-way done BRK instruction.
        this.polltime(4);
        let vector = 0xfffe;
        if (isIrq && this.nmi) {
            vector = 0xfffa;
            this.nmi = false;
        }
        this.takeInt = false;
        this.pc = this.readmem(vector) | (this.readmem(vector + 1) << 8);
        this.p.i = true;
        this.polltime(3);
    }

    branch(taken) {
        const offset = signExtend(this.getb());
        if (!taken) {
            this.polltime(1);
            this.checkInt();
            this.polltime(1);
            return;
        }
        const newPc = (this.pc + offset) & 0xffff;
        const pageCrossed = !!((this.pc & 0xff00) ^ (newPc & 0xff00));
        this.pc = newPc;
        if (!pageCrossed) {
            this.polltime(1);
            this.checkInt();
            this.polltime(2);
        } else {
            // 6502 polls twice during a taken branch with page
            // crossing and either is sufficient to trigger IRQ.
            // See https://wiki.nesdev.com/w/index.php/CPU_interrupts
            this.polltime(1);
            this.checkInt();
            const sawInt = this.takeInt;
            this.polltime(2);
            this.checkInt();
            this.takeInt |= sawInt;
            this.polltime(1);
        }
    }

    adcNonBCD(addend) {
        const result = this.a + addend + (this.p.c ? 1 : 0);
        this.p.v = !!((this.a ^ result) & (addend ^ result) & 0x80);
        this.p.c = !!(result & 0x100);
        this.a = result & 0xff;
        this.setzn(this.a);
    }

    // For flags and stuff see URLs like:
    // http://www.visual6502.org/JSSim/expert.html?graphics=false&a=0&d=a900f86911eaeaea&steps=16
    adcBCD(addend) {
        let ah = 0;
        const tempb = (this.a + addend + (this.p.c ? 1 : 0)) & 0xff;
        this.p.z = !tempb;
        let al = (this.a & 0xf) + (addend & 0xf) + (this.p.c ? 1 : 0);
        if (al > 9) {
            al -= 10;
            al &= 0xf;
            ah = 1;
        }
        ah += (this.a >>> 4) + (addend >>> 4);
        this.p.n = !!(ah & 8);
        this.p.v = !((this.a ^ addend) & 0x80) && !!((this.a ^ (ah << 4)) & 0x80);
        this.p.c = false;
        if (ah > 9) {
            this.p.c = true;
            ah -= 10;
            ah &= 0xf;
        }
        this.a = ((al & 0xf) | (ah << 4)) & 0xff;
    }

    // With reference to c64doc: http://vice-emu.sourceforge.net/plain/64doc.txt
    // and http://www.visual6502.org/JSSim/expert.html?graphics=false&a=0&d=a900f8e988eaeaea&steps=18
    sbcBCD(subend) {
        const carry = this.p.c ? 0 : 1;
        let al = (this.a & 0xf) - (subend & 0xf) - carry;
        let ah = (this.a >>> 4) - (subend >>> 4);
        if (al & 0x10) {
            al = (al - 6) & 0xf;
            ah--;
        }
        if (ah & 0x10) {
            ah = (ah - 6) & 0xf;
        }

        const result = this.a - subend - carry;
        this.p.n = !!(result & 0x80);
        this.p.z = !(result & 0xff);
        this.p.v = !!((this.a ^ result) & (subend ^ this.a) & 0x80);
        this.p.c = !(result & 0x100);
        this.a = al | (ah << 4);
    }

    adcBCDcmos(addend) {
        this.polltime(1); // One more cycle, apparently
        const carry = this.p.c ? 1 : 0;
        let al = (this.a & 0xf) + (addend & 0xf) + carry;
        let ah = (this.a >>> 4) + (addend >>> 4);
        if (al > 9) {
            al = (al - 10) & 0xf;
            ah++;
        }
        this.p.v = !((this.a ^ addend) & 0x80) && !!((this.a ^ (ah << 4)) & 0x80);
        this.p.c = false;
        if (ah > 9) {
            ah = (ah - 10) & 0xf;
            this.p.c = true;
        }
        this.a = this.setzn(al | (ah << 4));
    }

    sbcBCDcmos(subend) {
        this.polltime(1); // One more cycle, apparently
        const carry = this.p.c ? 0 : 1;
        const al = (this.a & 0xf) - (subend & 0xf) - carry;
        let result = this.a - subend - carry;
        if (result < 0) {
            result -= 0x60;
        }
        if (al < 0) result -= 0x06;

        this.adcNonBCD(subend ^ 0xff); // For flags
        this.a = this.setzn(result);
    }

    arr(arg) {
        // Insane instruction. I started with b-em source, but ended up using:
        // http://www.6502.org/users/andre/petindex/local/64doc.txt as reference,
        // tidying up as needed and fixing a couple of typos.
        if (this.p.d) {
            const temp = this.a & arg;

            const ah = temp >>> 4;
            const al = temp & 0x0f;

            this.p.n = this.p.c;
            this.a = (temp >>> 1) | (this.p.c ? 0x80 : 0x00);
            this.p.z = !this.a;
            this.p.v = (temp ^ this.a) & 0x40;

            if (al + (al & 1) > 5) this.a = (this.a & 0xf0) | ((this.a + 6) & 0xf);

            this.p.c = ah + (ah & 1) > 5;
            if (this.p.c) this.a = (this.a + 0x60) & 0xff;
        } else {
            this.a = this.a & arg;
            this.p.v = !!(((this.a >>> 7) ^ (this.a >>> 6)) & 0x01);
            this.a >>>= 1;
            if (this.p.c) this.a |= 0x80;
            this.setzn(this.a);
            this.p.c = !!(this.a & 0x40);
        }
    }
}

class FakeUserPort {
    write() {}
    read() {
        return 0xff;
    }
}


class DebugHook {
    constructor(cpu, functionName) {
        this.cpu = cpu;
        this.functionName = functionName;
        this.handlers = [];
    }
    add(handler) {
        const self = this;
        this.handlers.push(handler);
        if (!this.cpu[this.functionName]) {
            this.cpu[this.functionName] = function () {
                for (let i = 0; i < self.handlers.length; ++i) {
                    const handler = self.handlers[i];
                    if (handler.apply(handler, arguments)) {
                        self.cpu.stop();
                        return true;
                    }
                }
                return false;
            };
        }
        handler.remove = function () {
            self.remove(handler);
        };
        return handler;
    }
    remove(handler) {
        const i = this.handlers.indexOf(handler);
        if (i < 0) throw "Unable to find debug hook handler";
        this.handlers = this.handlers.slice(0, i).concat(this.handlers.slice(i + 1));
        if (this.handlers.length === 0) {
            this.cpu[this.functionName] = null;
        }
    }
    clear() {
        this.handlers = [];
        this.cpu[this.functionName] = null;
    }
}

export class Cpu6502 extends Base6502 {
    constructor(video_, music5000_) {
        super();
        this.video = video_;
        this.music5000 = music5000_;
        this.memStatOffsetByIFetchBank = new Uint32Array(16); // helps in master map of LYNNE for non-opcode read/writes
        this.memStatOffset = 0;
        this.memStat = new Uint8Array(512);
        this.memLook = new Int32Array(512); // Cannot be unsigned as we use negative offsets
        this.ramRomOs = new Uint8Array(128 * 1024 + 17 * 16 * 16384);
        this.romOffset = 128 * 1024;
        this.osOffset = this.romOffset + 16 * 16 * 1024;
        this.romsel = 0;
        this.acccon = 0;
        this.interrupt = 0;
        this.FEslowdown = [true, false, true, true, false, false, true, false];
        this.oldPcArray = new Uint16Array(256);
        this.oldAArray = new Uint8Array(256);
        this.oldXArray = new Uint8Array(256);
        this.oldYArray = new Uint8Array(256);
        this.oldPcIndex = 0;
        this.resetLine = true;
        this.cpuMultiplier = 1;
        this.videoCyclesBatch = 0;
        this.peripheralCyclesPerSecond = 2 * 1000 * 1000;
        this.JimPageSel = 0;
       
        this.peripheralCycles = 0;
        this.videoCycles = 0;

        this._debugRead = this._debugWrite = this._debugInstruction = null;
        this.debugInstruction = new DebugHook(this, "_debugInstruction");
        this.debugRead = new DebugHook(this, "_debugRead");
        this.debugWrite = new DebugHook(this, "_debugWrite");
    }

    getPrevPc(index) {
        return this.oldPcArray[(this.oldPcIndex - index) & 0xff];
    }

    romSelect(b) {
        this.romsel = b;
        const bankOffset = ((b & 15) << 14) + this.romOffset;
        const offset = bankOffset - 0x8000;
        for (let c = 128; c < 192; ++c) this.memLook[c] = this.memLook[256 + c] = offset;
        const swram = beebSwram[b & 15] ? 1 : 2;
        for (let c = 128; c < 192; ++c) this.memStat[c] = this.memStat[256 + c] = swram;
    }

    writeAcccon(b) {
        this.acccon = b;
        // ACCCON is
        // IRR TST IJF ITU  Y  X  E  D
        //  7   6   5   4   3  2  1  0
        // Video offset (to LYNNE) is controlled by the "D" bit of ACCCON.
        // LYNNE lives at 0xb000 in our map, but the offset we use here is 0x8000
        // as the video circuitry will already be looking at 0x3000 or so above
        // the offset.
        this.videoDisplayPage = b & 1 ? 0x8000 : 0x0000;

        const bitE = !!(b & 2);
        const bitX = !!(b & 4);
        const bitY = !!(b & 8);
        // The "X" bit controls the "illegal" paging 20KB region overlay of LYNNE.
        // This loop rewires which paged RAM 0x3000 - 0x7fff hits.
        for (let i = 48; i < 128; ++i) {
            // For "normal" access, it's simple: shadow or not.
            this.memLook[i] = bitX ? 0x8000 : 0;
            // For special Master opcode access at 0xc000 - 0xdfff,
            // it's more involved.
            if (bitY) {
                // If 0xc000 is mapped as RAM, the Master opcode access
                // is disabled; follow what normal access does.
                this.memLook[i + 256] = this.memLook[i];
            } else {
                // Master opcode access enabled; bit E determines whether
                // it hits shadow RAM or normal RAM. This is independent
                // of bit X.
                this.memLook[i + 256] = bitE ? 0x8000 : 0;
            }
        }
        // The "Y" bit pages in HAZEL at c000->dfff. HAZEL is mapped in our RAM
        // at 0x9000, so (0x9000 - 0xc000) = -0x3000 is needed as an offset.
        const hazelRAM = bitY ? 1 : 2;
        const hazelOff = bitY ? -0x3000 : this.osOffset - 0xc000;
        for (let i = 192; i < 224; ++i) {
            this.memLook[i] = this.memLook[i + 256] = hazelOff;
            this.memStat[i] = this.memStat[i + 256] = hazelRAM;
        }
    }

    // Works for unpaged RAM only (ie stack and zp)
    readmemZpStack(addr) {
        addr &= 0xffff;
        const res = this.ramRomOs[addr];
        if (this._debugRead) this._debugRead(addr, 0, res);
        return res | 0;
    }
    writememZpStack(addr, b) {
        addr &= 0xffff;
        b |= 0;
        if (this._debugWrite) this._debugWrite(addr, b);
        this.ramRomOs[addr] = b;
    }

    // Handy debug function to read a string zero or \n terminated.
    readString(addr) {
        let s = "";
        for (;;) {
            const b = this.readmem(addr);
            addr++;
            if (b === 0 || b === 13) break;
            s += String.fromCharCode(b);
        }
        return s;
    }

    findString(string, addr) {
        addr = addr | 0;
        for (; addr < 0xffff; ++addr) {
            let i;
            for (i = 0; i < string.length; ++i) {
                if (this.readmem(addr + i) !== string.charCodeAt(i)) break;
            }
            if (i === string.length) {
                return addr;
            }
        }
        return null;
    }

    readArea(addr, len) {
        let str = "";
        for (let i = 0; i < len; ++i) {
            str += utils.hexbyte(this.readmem(addr + i));
        }
        return str;
    }

    is1MHzAccess(addr) {
        addr &= 0xffff;
        return addr >= 0xfc00 && addr < 0xff00 && (addr < 0xfe00 || this.FEslowdown[(addr >>> 5) & 7]);
    }

    readDevice(addr) {
        addr &= 0xffff;

        if (addr === 0xfcff) {
            return this.JimPageSel;
        }

        if ((this.JimPageSel & 0xf0) === 0x30 && (addr & 0xff00) === 0xfd00) {
            return this.music5000.read(this.JimPageSel, addr);
        }
    
        switch (addr & ~0x0003) {
            case 0xfc10:
                return this.teletextAdaptor.read(addr - 0xfc10);
            case 0xfc20:
            case 0xfc24:
            case 0xfc28:
            case 0xfc2c:
            case 0xfc30:
            case 0xfc34:
            case 0xfc38:
            case 0xfc3c:
                // SID Chip.
                break;
            case 0xfc40:
            case 0xfc44:
            case 0xfc48:
            case 0xfc4c:
            case 0xfc50:
            case 0xfc54:
            case 0xfc58:
            case 0xfc5c:
                // IDE
                break;
            case 0xfe00:
            case 0xfe04:
                return this.crtc.read(addr);
            case 0xfe08:
            case 0xfe0c:
            case 0xfe10:
            case 0xfe14:
            case 0xfe18:
                return 42;
            case 0xfe20:
                break;
            case 0xfe24:
            case 0xfe28:
                break;
            case 0xfe30:
                break;
            case 0xfe34:
                break;
            case 0xfe38:
                break;
            case 0xfe3c:
                break;
            case 0xfe40:
            case 0xfe44:
            case 0xfe48:
            case 0xfe4c:
            case 0xfe50:
            case 0xfe54:
            case 0xfe58:
            case 0xfe5c:
                return this.sysvia.read(addr);
            case 0xfe60:
            case 0xfe64:
            case 0xfe68:
            case 0xfe6c:
            case 0xfe70:
            case 0xfe74:
            case 0xfe78:
            case 0xfe7c:
                return this.uservia.read(addr);
            case 0xfe80:
            case 0xfe84:
            case 0xfe88:
            case 0xfe8c:
            case 0xfe90:
            case 0xfe94:
            case 0xfe98:
            case 0xfe9c:
                return this.fdc.read(addr);
            case 0xfea0:
                break;
            case 0xfec0:
            case 0xfec4:
            case 0xfec8:
            case 0xfecc:
            case 0xfed0:
            case 0xfed4:
            case 0xfed8:
            case 0xfedc:
                break;
            case 0xfee0:
            case 0xfee4:
            case 0xfee8:
            case 0xfeec:
            case 0xfef0:
            case 0xfef4:
            case 0xfef8:
            case 0xfefc:
//                return this.tube.read(addr);
        }
        if (addr >= 0xfc00 && addr < 0xfe00) return 0xff;
        return addr >>> 8;
    }

    videoRead(addr) {
        return this.ramRomOs[addr | this.videoDisplayPage] | 0;
    }

    readmem(addr) {
        addr &= 0xffff;
        if (this.memStat[this.memStatOffset + (addr >>> 8)]) {
            const offset = this.memLook[this.memStatOffset + (addr >>> 8)];
            const res = this.ramRomOs[offset + addr];
            if (this._debugRead) this._debugRead(addr, res, offset);
            return res | 0;
        } else {
            const res = this.readDevice(addr);
            if (this._debugRead) this._debugRead(addr, res, 0);
            return res | 0;
        }
    }

    peekmem(addr) {
        if (this.memStat[this.memStatOffset + (addr >>> 8)]) {
            const offset = this.memLook[this.memStatOffset + (addr >>> 8)];
            return this.ramRomOs[offset + addr];
        } else {
            return 0xff; // TODO; peekDevice -- this.peekDevice(addr);
        }
    }

    writemem(addr, b) {
        addr &= 0xffff;
        b |= 0;
        if (this._debugWrite) this._debugWrite(addr, b);
        if (this.memStat[this.memStatOffset + (addr >>> 8)] === 1) {
            const offset = this.memLook[this.memStatOffset + (addr >>> 8)];
            this.ramRomOs[offset + addr] = b;
            return;
        }
        if (addr < 0xfc00 || addr >= 0xff00) return;
        this.writeDevice(addr, b);
    }
    writeDevice(addr, b) {
        b |= 0;

        if (addr === 0xfcff) {
            this.JimPageSel = b;
            return;
        }

        if ((this.JimPageSel & 0xf0) === 0x30 && (addr & 0xff00) === 0xfd00) {
            this.music5000.write(this.JimPageSel, addr, b);
            return;
        }
    
        switch (addr & ~0x0003) {
            case 0xfc10:
                return this.teletextAdaptor.write(addr - 0xfc10, b);
            case 0xfc20:
            case 0xfc24:
            case 0xfc28:
            case 0xfc2c:
            case 0xfc30:
            case 0xfc34:
            case 0xfc38:
            case 0xfc3c:
                // SID chip
                break;
            case 0xfc40:
            case 0xfc44:
            case 0xfc48:
            case 0xfc4c:
            case 0xfc50:
            case 0xfc54:
            case 0xfc58:
            case 0xfc5c:
                // IDE
                break;
            case 0xfe00:
            case 0xfe04:
                return this.crtc.write(addr, b);
            case 0xfe08:
            case 0xfe0c:
            case 0xfe10:
            case 0xfe14:
            case 0xfe18:
                break;
            case 0xfe20:
                return this.ula.write(addr, b);
            case 0xfe24:
            case 0xfe28:
                return this.ula.write(addr, b);
            case 0xfe2c:
                return this.ula.write(addr, b);
            case 0xfe30:
                return this.romSelect(b);
            case 0xfe34:
                return this.romSelect(b);
            case 0xfe38:
                break;
            case 0xfe3c:
                return this.romSelect(b);
            case 0xfe40:
            case 0xfe44:
            case 0xfe48:
            case 0xfe4c:
            case 0xfe50:
            case 0xfe54:
            case 0xfe58:
            case 0xfe5c:
                return this.sysvia.write(addr, b);
            case 0xfe60:
            case 0xfe64:
            case 0xfe68:
            case 0xfe6c:
            case 0xfe70:
            case 0xfe74:
            case 0xfe78:
            case 0xfe7c:
                return this.uservia.write(addr, b);
            case 0xfe80:
            case 0xfe84:
            case 0xfe88:
            case 0xfe8c:
            case 0xfe90:
            case 0xfe94:
            case 0xfe98:
            case 0xfe9c:
                return this.fdc.write(addr, b);
            case 0xfec0:
            case 0xfec4:
            case 0xfec8:
            case 0xfecc:
            case 0xfed0:
            case 0xfed4:
            case 0xfed8:
            case 0xfedc:
                break;
            case 0xfee0:
            case 0xfee4:
            case 0xfee8:
            case 0xfeec:
            case 0xfef0:
            case 0xfef4:
            case 0xfef8:
            case 0xfefc:
            //    return this.tube.write(addr, b);
        }
    }

    async loadRom(name, offset) {
        if (name.indexOf("http") !== 0) name = "roms/" + name;
        console.log("Loading ROM from " + name);
        const ramRomOs = this.ramRomOs;
        let data = await utils.loadData(name);
        if (/\.zip/i.test(name)) {
            data = utils.unzipRomImage(data).data;
        }
        const len = data.length;
        if (len !== 16384 && len !== 8192) {
            throw new Error("Broken rom file");
        }
        for (let i = 0; i < len; ++i) {
            ramRomOs[offset + i] = data[i];
        }
    }

    async loadOs(os) {
        const extraRoms = Array.prototype.slice.call(arguments, 1);
        os = "roms/" + os;
        console.log("Loading OS from " + os);
        const ramRomOs = this.ramRomOs;
        const data = await utils.loadData(os);
        const len = data.length;
        if (len < 16384 || len & 16383) throw new Error("Broken ROM file (length=" + len + ")");
        for (let i = 0; i < 16384; ++i) {
            ramRomOs[this.osOffset + i] = data[i];
        }
        const numExtraBanks = (len - 16384) / 16384;
        let romIndex = 16 - numExtraBanks;
        for (let i_1 = 0; i_1 < numExtraBanks; ++i_1) {
            const srcBase = 16384 + 16384 * i_1;
            const destBase = this.romOffset + (romIndex + i_1) * 16384;
            for (let j = 0; j < 16384; ++j) {
                ramRomOs[destBase + j] = data[srcBase + j];
            }
        }
        const awaiting = [];
        for (let i_2 = 0; i_2 < extraRoms.length; ++i_2) {
            // Skip over banks 4-7 (sideways RAM on a Master)
            romIndex--;
            while (beebSwram[romIndex]) {
                romIndex--;
            }

            awaiting.push(this.loadRom(extraRoms[i_2], this.romOffset + romIndex * 16384));
        }
        return await Promise.all(awaiting);
    }

    setReset(resetOn) {
        this.resetLine = !resetOn;
    }

    reset(hard) {
        if (hard) {
            for (let i = 0; i < 16; ++i) this.memStatOffsetByIFetchBank[i] = 0;
         
            for (let i = 0; i < 128; ++i) this.memStat[i] = this.memStat[256 + i] = 1;
            for (let i = 128; i < 256; ++i) this.memStat[i] = this.memStat[256 + i] = 2;
            for (let i = 0; i < 128; ++i) this.memLook[i] = this.memLook[256 + i] = 0;
            for (let i = 128; i < 192; ++i) this.memLook[i] = this.memLook[256 + i] = this.romOffset - 0x8000;
            for (let i = 192; i < 256; ++i) this.memLook[i] = this.memLook[256 + i] = this.osOffset - 0xc000;
            for (let i = 0xfc; i < 0xff; ++i) this.memStat[i] = this.memStat[256 + i] = 0;
         
            // DRAM content is not guaranteed to contain any particular
            // value on start up, so we choose values that help avoid
            // bugs in various games.
            for (let i = 0; i < this.romOffset; ++i) {
                if (i < 0x100) {
                    // For Clogger.
                    this.ramRomOs[i] = 0x00;
                } else {
                    // For Eagle Empire.
                    this.ramRomOs[i] = 0x00; // JR was 0xff;
                }
            }
            this.videoDisplayPage = 0;
            this.scheduler = new Scheduler();
            this.sysvia = via.SysVia(
                this,
                this.video
            );
            this.uservia = via.UserVia(this);
            this.fdc = new WD1770(this, this.scheduler);
            this.crtc = this.video.crtc;
            this.ula = this.video.ula;
            this.teletextAdaptor = new TeletextAdaptor(this);
            this.sysvia.reset(hard);
            this.uservia.reset(hard);
        }
        if (hard) {
            this.targetCycles = 0;
            this.currentCycles = 0;
            this.cycleSeconds = 0;
        }
        this.pc = this.readmem(0xfffc) | (this.readmem(0xfffd) << 8);
        this.p = new Flags();
        this.p.i = true;
        this.nmi = false;
        this.halted = false;
        this.JimPageSel = 0;
        this.video.reset(this, this.sysvia, hard);
        this.teletextAdaptor.reset(hard);
        this.music5000.reset(hard);
    }

    polltimeAddr(cycles, addr) {
        cycles = cycles | 0;
        if (this.is1MHzAccess(addr)) {
            cycles += 1 + ((cycles ^ this.currentCycles) & 1);
        }
        this.polltime(cycles);
    }

    polltime(cycles) {
        cycles |= 0;
        this.currentCycles += cycles;
        this.video.polltime(cycles);
        this.sysvia.polltime(cycles);
        this.uservia.polltime(cycles);
        this.scheduler.polltime(cycles);
        this.teletextAdaptor.polltime(cycles);
        this.music5000.polltime(cycles);
    }

    execute(numCyclesToRun) {
        this.halted = false;
        this.targetCycles += numCyclesToRun;
        // To prevent issues with wrapping around / overflowing the accuracy that poxy Javascript numbers have,
        // find the smaller of the target and current cycles, and if that's over one second's worth; subtract
        // that from both, to keep the domain low (while accumulating seconds). Take care to preserve the bottom
        // bit though; as that encodes whether we're on an even or odd bus cycle.
        const smaller = Math.min(this.targetCycles, this.currentCycles) & 0xfffffffe;
        if (smaller >= 2 * 1000 * 1000) {
            this.targetCycles -= 2 * 1000 * 1000;
            this.currentCycles -= 2 * 1000 * 1000;
            this.cycleSeconds++;
        }

        while (!this.halted && this.currentCycles < this.targetCycles) {
            this.memStatOffset = this.memStatOffsetByIFetchBank[this.pc >>> 12];
            const opcode = this.readmem(this.pc);
            this.incpc();
            this.runner.run(opcode);
            if (this.takeInt) this.brk(true);
            if (!this.resetLine) this.reset(false);
        }
        return !this.halted;
    }

    stop() {
        this.halted = true;
    }

    async initialise() {
        await this.loadOs.apply(this, ["OS.rom", "BASIC.ROM", "DFS-2.26.rom", "WWP-1.49.rom", "EDIT.rom", "AMPLE.rom", "ATS-3.0.rom", "ADT-1.40.rom"]);
        this.reset(true);
    }
}
