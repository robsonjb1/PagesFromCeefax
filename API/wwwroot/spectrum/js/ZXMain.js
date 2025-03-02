"use strict";

class ZXMain
{
    constructor(canvasIdForScreen, imageCatalogue)
    {
        const self = this;

        // core is a set of callbacks passed to the Z80 emulator
        // for giving an environment to the processor:
        // - a memory space for reading / writing
        // - I/O ports for reading / writing
        const core = {
            mem_read  : function(addr)     { return self._mem_read (addr)    ; },
            mem_write : function(addr,val) {        self._mem_write(addr,val); },
            io_read   : function(port)     { return self._io_read  (port)    ; },
            io_write  : function(port,val) {        self._io_write (port,val); },
        };

        // create the Z80 emulator object
        this.cpu = new Z80(core);

        // create a byte array for holding 64 KB of memory and initialize it to zero
        this.mem = new Uint8Array(65536);
        for (let i = 0; i < 63336; i++)
            this.mem[i] = 0;

        this._screen = new ZXScreen(canvasIdForScreen);
        this._keyb = new ZXKeyboard(window, this);
        this._sound = new ZXSoundOutput();
        this._imageCatalogue = imageCatalogue;

        // previous sound bit for reacting only to changes in bit
        this._prev_sound_bit = 0;

        // initially not started
        this._started = false;
    }

    start()
    {
        // don't start more than once
        if (this._started) return;

        this._start();
    }

    ////////////////////////////////////////////////////////////////////////////////
    // CORE: memory R/W, ports R/W
    ////////////////////////////////////////////////////////////////////////////////

    // read from memory at address addr, return byte at that position
    _mem_read(addr) {
        let val = this.mem[addr];
        this._emulate_contended_memory(addr, false);
        return val;
    }

    // write to memory at address addr, putting byte at that position
    _mem_write(addr, val) {
        // make ROM read-only
        if (addr >= 0x4000)
            this.mem[addr] = val;
        this._emulate_contended_memory(addr, true);
    }

    // read from Input port
    _io_read(port) {
        let val = 0xFF;
        // ULA responds to any even address
        if ((port & 1) == 0) {
            // read from keyboard
            val = this._keyb.getKeyboardValueForPort(port);
        }
        else
            val = 0;

        return val;
    }

    // write to output port
    _io_write(port, val) {
        // ULA responds to any even address
        if ((port & 1) == 0) {
            // border is set with lower 3 bits of value
            this._screen.border = val & 0x07;
            // sound is bit 4
            const sound_bit = val & 0x10 ? 1 : 0;
            if (sound_bit != this._prev_sound_bit) {
                // calculate transition time
                let ttime = this._framecount * this._frametime;
                ttime += this._cyclecount / this._cpufreq;
                // notify transition time
                this._sound.notifyTransitionTime(ttime);
                // annotate previous bit
                this._prev_sound_bit = sound_bit;
            }
        }
    }

    // reset the processor
    reset() {
        this.cpu.reset();
    }

    // emulate contended memory
    _emulate_contended_memory(addr, isWrite) {
        if (addr < 0x4000 || addr >= 0x8000)
            return;

        if (this._emulate_contended_memory_special_cases(isWrite))
            return;

        this._cyclecount += this._ctmem_extra_cycles(this._cyclecount);
    }

    // special cases for some tricky instructions (incomplete and inexact)
    _emulate_contended_memory_special_cases(isWrite) {
        const state = this.cpu.getState();
        const pc = state.pc;
        const icurr = this.mem[pc];
        const iprev = this.mem[pc-1];
        if (!isWrite) {
            if (iprev == 0xED) {
                if (icurr == 0xB1 || icurr == 0xB9) {   // CPIR, CPDR
                    this._emulate_1xN_wait_states(5);
                    return true;
                }
                if (icurr == 0xB3 || icurr == 0xBB) {   // OTIR, OTDR
                    this._emulate_1xN_wait_states(5);
                    return true;
                }
            }
            if (iprev == 0x10 || iprev == 0x20 || iprev == 0x30 ||  // DJNZ, JR*
                iprev == 0x18 || iprev == 0x28 || iprev == 0x38) {
                this._emulate_1xN_wait_states(5); 
                return true;
            }
        }
        if (isWrite) {
            if (iprev == 0xED) {
                if (icurr == 0xA0 || icurr == 0xA8) {   // LDI, LDD
                    this._emulate_1xN_wait_states(2);
                    return true;
                }
                if (icurr == 0xB0 || icurr == 0xB8) {   // LDIR, LDDR
                    this._emulate_1xN_wait_states(7);
                    return true;
                }
                if (icurr == 0xB2 || icurr == 0xBA) {   // INIR, INDR
                    this._emulate_1xN_wait_states(5);
                    return true;
                }
            }
        }
        return false;
    }

    // emulation for 1xN special cases
    _emulate_1xN_wait_states(N) {
        for (let i = 0; i < N; i++) {
            this._cyclecount++;
            this._cyclecount += this._ctmem_extra_cycles(this._cyclecount);
        }
        this._cyclecount -= (N-1);
    }

    // reference: https://worldofspectrum.org/faq/reference/48kreference.htm#ZXSpectrum
    // from paragraph which starts with "The 50 Hz interrupt is synchronized with..."
    // if you only read from https://worldofspectrum.org/faq/reference/48kreference.htm#Contention
    // without reading the previous paragraphs about line timings, it may be confusing.
    _ctmem_extra_cycles(T)
    {
        //return 3;
        const wait_pattern = [6, 5, 4, 3, 2, 1, 0, 0];
        T += 1;
        const line = (T / 224) | 0; // fast float->int
        if (line >= 64 && line < 256) {
            const halfpix = T % 224;
            if (halfpix < 128) {
                const wpi = halfpix % 8;
                return wait_pattern[wpi];
            }
        }
        return 0;
    }


    ////////////////////////////////////////////////////////////////////////////////
    // Animation loop and timing
    ////////////////////////////////////////////////////////////////////////////////

    _start()
    {
        this._cpufreq = 3500;       // Spectrum clock frequency 3.5mHz
        this._framecount = 0;
        this._frametime = 20;       // refresh display and run CPU cycles every 20ms
        this._prevtime = null;

        // accumulated time, for emitting interrupt once per frame
        this._accumtime = this._frametime;

        // flash state variables
        this._flashstate  = false;  // initially not inverted
        this._flashtime   = 0;      // timestamp for inverting
        this._flashperiod = 320;    // flash period in ms

        // cycle counter and period (cycles per frame) are needed for accurate sound
        this._cyclecount = 0;
        this._cycleperiod;

        // game SNA image list and banner
        this._imageIndex = 0;
        this._bannerPeriod = 100;
        this._bannerTime = this._bannerPeriod;

        // Load the SNA files into the image array
        for (let i = 0; i < this._imageCatalogue.length; i++)
        {
            const self = this;
            loadRemoteBinaryFile('sna/' + this._imageCatalogue[i].file, function(data) {
                self._imageCatalogue[i].data = data;
                if(i==0) { self.loadSNA(data); }
            });
        }

        // Load OS ROM and start emulation once ROM is loaded
        const self = this;
        loadRemoteBinaryFile('sna/zx48.sna', function(data) {
            for (let i = 0; i < 0x4000; i++) {
                self.mem[i] = data[i];
            }
            // Request first animation frame draw
            self._requestAnimation();
        });
    }

    _requestAnimation()
    {
        const self = this;
        requestAnimationFrame(function(time) {
            self._onAnimationFrame(time);
        });
    }

    _onAnimationFrame(time)
    {
        if (this._prevtime != null)
        {
            // deltatime is current timestamp minus previous
            let deltatime = time - this._prevtime;

            // detect abnormal frame time (maybe due to window hidden, or other cases)
            if (deltatime > 500)
                deltatime = this._frametime;

            // draw frame for given deltatime
            this._onDrawFrame(deltatime);
        }
        // annotate previous time
        this._prevtime = time;

        // request another frame
        this._requestAnimation();
    }

    _onDrawFrame(deltatime)
    {
        this._accumtime += deltatime;

        // Calculate number of cycles for given frequency and frame time
        this._cycleperiod = this._cpufreq * this._frametime;

        // if accumtime exceeds deltatime, we must draw
        while (this._accumtime >= this._frametime)
        {
            let numins = 0;
            // execute instructions until max cycle count reached...
            while (this._cyclecount < this._cycleperiod) {
                // pitfall: this._cyclecount += this.cpu.run_instruction();
                // will fail to consider cycles incremented as a side effect
                // of running run_instruction (ex: contended memory)
                let instructionCycles = this.cpu.run_instruction();
                this._cyclecount += instructionCycles;
                numins++;
                // ... or CPU halted (with HALT instruction)
                if (this.cpu.is_halted()) {
                    break;
                }    
            }

            // cycle count must be monotonic
            this._cyclecount -= this._cycleperiod;
            
            // evaluate flash inversion flag
            this._flashtime += this._frametime;
            if (this._flashtime >= this._flashperiod) {
                this._flashtime -= this._flashperiod;
                this._flashstate = !this._flashstate;
            }

            // create banner text and redraw screen
            let bannerText = (this._imageIndex + 1) + " " + this._imageCatalogue[this._imageIndex].name;
            this._screen.update(this.mem, this._flashstate, this._bannerTime > 0 ? bannerText : "");
            this._bannerTime-- & 255;
           
            // emit maskable interrupt to wake up CPU from halted state
            this.cpu.interrupt(false, 0);

            // increase frame counter
            this._framecount++;

            // substract frametime from accumtime before loop restart
            this._accumtime -= this._frametime;
        }
    }

    // documentation for SNA format: https://faqwiki.zxnet.co.uk/wiki/SNA_format
    loadSNA(data)
    {
        // helper for creating word from 2 bytes
        function mkword(lobyte, hibyte) { return 256*hibyte + lobyte; }

        // helper for loading flags from byte to object. reference: https://www.istvannovak.net/2018/02/01/zx-spectrum-ide-part-5-implementing-z80-instructions-1/
        function objForByte(b) {
            let o = {};
            const flagnames = "CNPXHYZS"; // flag names, bit 0 is C, bit 1 is N... bit 7 is S.
            for (let i = 0; i < 8; i++)
                o[flagnames[i]] = (b & (1 << i)) ? 1 : 0;
            return o;
        }

        // first 27 bytes hold register state, restore it
        const state = this.cpu.getState();
        state.pc = 0x0072;  // RETN in ROM, see SNA documentation
        state.i       = data[0x00];
        state.l_prime = data[0x01];
        state.h_prime = data[0x02];
        state.e_prime = data[0x03];
        state.d_prime = data[0x04];
        state.c_prime = data[0x05];
        state.b_prime = data[0x06];
        state.flags_prime = objForByte(data[0x07]);
        state.a_prime = data[0x08];
        state.l       = data[0x09];
        state.h       = data[0x0A];
        state.e       = data[0x0B];
        state.d       = data[0x0C];
        state.c       = data[0x0D];
        state.b       = data[0x0E];
        state.iy      = mkword(data[0x0F], data[0x10]);
        state.ix      = mkword(data[0x11], data[0x12]);
        state.iff2    = data[0x13];
        state.r       = data[0x14];
        state.flags = objForByte(data[0x15]);
        state.a       = data[0x16];
        state.sp      = mkword(data[0x17], data[0x18]);
        state.imode   = data[0x19];
        this.cpu.setState(state);

        // last byte holds border state
        this._screen.border  = data[0x1A];

        // copy 48KB of data from snapshot to RAM memory
        const datalen = 0xC000; // 48K
        const dataoff = 0x1B;   // 27
        const memoff  = 0x4000;  // 16K
        for (let i = 0; i < datalen; i++) {
            this.mem[memoff+i] = data[dataoff+i];
        }
    }
  
    saveSNA()
    {
        // CPU state
        const state = this.cpu.getState();
        // stack pointer must be in RAM
        if (state.sp < 0x4000 + 2)
            return null;
            
        // create array for canonic SNAs are 49152 (48KB) + 27 bytes long.
        const data = new Array(49179);

        // helper for getting low and high bytes from word
        function lobyte(word) { return  word       & 0xFF; }
        function hibyte(word) { return (word >> 8) & 0xFF; }

        // helper for loading flags from object to byte.
        function byteForObj(o) {
            let b = 0;
            const flagnames = "CNPXHYZS"; // flag names, bit 0 is C, bit 1 is N... bit 7 is S.
            for (let i = 0; i < 8; i++)
                b |= o[flagnames[i]] ? (1 << i) : 0;
            return b;
        }

        // first 27 bytes hold register state, save it
        data[0x00] = state.i;
        data[0x01] = state.l_prime;
        data[0x02] = state.h_prime;
        data[0x03] = state.e_prime;
        data[0x04] = state.d_prime;
        data[0x05] = state.c_prime;
        data[0x06] = state.b_prime;
        data[0x07] = byteForObj(state.flags_prime);
        data[0x08] = state.a_prime;
        data[0x09] = state.l;
        data[0x0A] = state.h;
        data[0x0B] = state.e;
        data[0x0C] = state.d;
        data[0x0D] = state.c;
        data[0x0E] = state.b;
        data[0x0F] = lobyte(state.iy);
        data[0x10] = hibyte(state.iy);
        data[0x11] = lobyte(state.ix);
        data[0x12] = hibyte(state.ix);
        data[0x13] = state.iff2;
        data[0x14] = state.r;
        data[0x15] = byteForObj(state.flags);
        data[0x16] = state.a;
        // store stack pointer with value decreased, we will be pushing PC onto stack
        data[0x17] = lobyte(state.sp - 2);
        data[0x18] = hibyte(state.sp - 2);
        data[0x19] = state.imode;

        // last byte holds border state
        data[0x1A] = this._screen.border;

        // copy 48KB of data from RAM memory to snapshot
        const datalen = 0xC000; // 48K
        const dataoff = 0x1B;   // 27
        const memoff  = 0x4000;  // 16K
        for (let i = 0; i < datalen; i++) {
            data[dataoff+i] = this.mem[memoff+i];
        }

        // push PC onto stack (directly to snapshot RAM)
        data[dataoff - memoff + state.sp - 2] = lobyte(state.pc);
        data[dataoff - memoff + state.sp - 1] = hibyte(state.pc);

        return data;
    }
  
    imageMove(delta)
    {
        // Save the current image
        this._imageCatalogue[this._imageIndex].data = this.saveSNA();
        
        // Move to next image
        this._imageIndex = this._imageIndex + delta;
        if(this._imageIndex == this._imageCatalogue.length) { this._imageIndex = 0; }
        if(this._imageIndex < 0) { this._imageIndex = this._imageCatalogue.length - 1; }
        
        // Display new channel banner
        this._bannerTime = this._bannerPeriod;
        this.loadSNA(this._imageCatalogue[this._imageIndex].data);

        // Apply any pokes if required
        if(this._imageCatalogue[this._imageIndex].poke)
        {
            for(let i=0; i<this._imageCatalogue[this._imageIndex].poke.length; i++)
            {
                this.mem[this._imageCatalogue[this._imageIndex].poke[i].loc] =
                    this._imageCatalogue[this._imageIndex].poke[i].val;
            }
        }
    }
}
