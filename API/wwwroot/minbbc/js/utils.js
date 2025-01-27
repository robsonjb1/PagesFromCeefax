"use strict";


export function isFirefox() {
    // With thanks to http://stackoverflow.com/questions/9847580/how-to-detect-safari-chrome-ie-firefox-and-opera-browser
    // Opera 8.0+ (UA detection to detect Blink/v8-powered Opera)
    return typeof InstallTrigger !== "undefined"; // Firefox 1.0+
}

export function parseAddr(s) {
    if (s[0] === "$" || s[0] === "&") return parseInt(s.substr(1), 16);
    if (s.indexOf("0x") === 0) return parseInt(s.substr(2), 16);
    return parseInt(s, 16);
}

export const userKeymap = [];

export const BBC = {
    SEMICOLON_PLUS: [7, 5],
    MINUS: [7, 1],
    LEFT_SQUARE_BRACKET: [8, 3],
    RIGHT_SQUARE_BRACKET: [8, 5],
    COMMA: [6, 6],
    PERIOD: [7, 6],
    SLASH: [8, 6],
    SHIFTLOCK: [0, 5],
    TAB: [0, 6],
    RETURN: [9, 4],
    DELETE: [9, 5],
    COPY: [9, 6],
    SHIFT: [0, 0],
    ESCAPE: [0, 7],
    CTRL: [1, 0],
    CAPSLOCK: [0, 4],
    LEFT: [9, 1],
    UP: [9, 3],
    RIGHT: [9, 7],
    DOWN: [9, 2],
    K0: [7, 2],
    K1: [0, 3],
    K2: [1, 3],
    K3: [1, 1],
    K4: [2, 1],
    K5: [3, 1],
    K6: [4, 3],
    K7: [4, 2],
    K8: [5, 1],
    K9: [6, 2],

    Q: [0, 1],
    W: [1, 2],
    E: [2, 2],
    R: [3, 3],
    T: [3, 2],
    Y: [4, 4],
    U: [5, 3],
    I: [5, 2],
    O: [6, 3],
    P: [7, 3],

    A: [1, 4],
    S: [1, 5],
    D: [2, 3],
    F: [3, 4],
    G: [3, 5],
    H: [4, 5],
    J: [5, 4],
    K: [6, 4],
    L: [6, 5],

    Z: [1, 6],
    X: [2, 4],
    C: [2, 5],
    V: [3, 6],
    B: [4, 6],
    N: [5, 5],
    M: [5, 6],

    F0: [0, 2],
    F1: [1, 7],
    F2: [2, 7],
    F3: [3, 7],
    F4: [4, 1],
    F5: [4, 7],
    F6: [5, 7],
    F7: [6, 1],
    F8: [6, 7],
    F9: [7, 7],

    SPACE: [2, 6],

    UNDERSCORE_POUND: [8, 2],
    AT: [7, 4],
    COLON_STAR: [8, 4],
    PIPE_BACKSLASH: [8, 7],
    HAT_TILDE: [8, 1],

    // row 1
    NUMPADPLUS: [10, 3],
    NUMPADMINUS: [11, 3],
    NUMPADSLASH: [10, 4],
    NUMPADASTERISK: [11, 5],

    // row 2
    NUMPAD7: [11, 1],
    NUMPAD8: [10, 2],
    NUMPAD9: [11, 2],
    NUMPADHASH: [10, 5],
    // row 3
    NUMPAD4: [10, 7],
    NUMPAD5: [11, 7],
    NUMPAD6: [10, 1],
    NUMPAD_DELETE: [11, 4],
    //row4
    NUMPAD1: [11, 6],
    NUMPAD2: [12, 7],
    NUMPAD3: [12, 6],
    NUMPADCOMMA: [12, 5],

    //row 5
    NUMPAD0: [10, 6],
    NUMPAD_DECIMAL_POINT: [12, 4],
    NUMPADENTER: [12, 3],
};

export function stringToBBCKeys(str) {
    const array = [];
    let shiftState = false;
    let capsLockState = true;
    for (let i = 0; i < str.length; ++i) {
        const c = str.charCodeAt(i);
        let charStr = str.charAt(i);
        let bbcKey = null;
        let needsShift = false;
        let needsCapsLock = true;
        if (c >= 65 && c <= 90) {
            // A-Z
            bbcKey = BBC[charStr];
        } else if (c >= 97 && c <= 122) {
            // a-z
            charStr = String.fromCharCode(c - 32);
            bbcKey = BBC[charStr];
            needsCapsLock = false;
        } else if (c >= 48 && c <= 57) {
            // 0-9
            bbcKey = BBC["K" + charStr];
        } else if (c >= 33 && c <= 41) {
            // ! to )
            charStr = String.fromCharCode(c + 16);
            bbcKey = BBC["K" + charStr];
            needsShift = true;
        } else {
            switch (charStr) {
                case "\n":
                    bbcKey = BBC.RETURN;
                    break;
                case "\t":
                    bbcKey = BBC.TAB;
                    break;
                case " ":
                    bbcKey = BBC.SPACE;
                    break;
                case "-":
                    bbcKey = BBC.MINUS;
                    break;
                case "=":
                    bbcKey = BBC.MINUS;
                    needsShift = true;
                    break;
                case "^":
                    bbcKey = BBC.HAT_TILDE;
                    break;
                case "~":
                    bbcKey = BBC.HAT_TILDE;
                    needsShift = true;
                    break;
                case "\\":
                    bbcKey = BBC.PIPE_BACKSLASH;
                    break;
                case "|":
                    bbcKey = BBC.PIPE_BACKSLASH;
                    needsShift = true;
                    break;
                case "@":
                    bbcKey = BBC.AT;
                    break;
                case "[":
                    bbcKey = BBC.LEFT_SQUARE_BRACKET;
                    break;
                case "{":
                    bbcKey = BBC.LEFT_SQUARE_BRACKET;
                    needsShift = true;
                    break;
                case "_":
                    bbcKey = BBC.UNDERSCORE_POUND;
                    break;
                case ";":
                    bbcKey = BBC.SEMICOLON_PLUS;
                    break;
                case "+":
                    bbcKey = BBC.SEMICOLON_PLUS;
                    needsShift = true;
                    break;
                case ":":
                    bbcKey = BBC.COLON_STAR;
                    break;
                case "*":
                    bbcKey = BBC.COLON_STAR;
                    needsShift = true;
                    break;
                case "]":
                    bbcKey = BBC.RIGHT_SQUARE_BRACKET;
                    break;
                case "}":
                    bbcKey = BBC.RIGHT_SQUARE_BRACKET;
                    needsShift = true;
                    break;
                case ",":
                    bbcKey = BBC.COMMA;
                    break;
                case "<":
                    bbcKey = BBC.COMMA;
                    needsShift = true;
                    break;
                case ".":
                    bbcKey = BBC.PERIOD;
                    break;
                case ">":
                    bbcKey = BBC.PERIOD;
                    needsShift = true;
                    break;
                case "/":
                    bbcKey = BBC.SLASH;
                    break;
                case "?":
                    bbcKey = BBC.SLASH;
                    needsShift = true;
                    break;
            }
        }

        if (!bbcKey) continue;

        if ((needsShift && !shiftState) || (!needsShift && shiftState)) {
            array.push(BBC.SHIFT);
            shiftState = !shiftState;
        }
        if ((needsCapsLock && !capsLockState) || (!needsCapsLock && capsLockState)) {
            array.push(BBC.CAPSLOCK);
            capsLockState = !capsLockState;
        }
        array.push(bbcKey);
    }

    if (shiftState) array.push(BBC.SHIFT);
    if (!capsLockState) array.push(BBC.CAPSLOCK);
    return array;
}

/**
 * Useful references:
 * http://www.cambiaresearch.com/articles/15/javascript-char-codes-key-codes
 * https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent.keyCode
 */
export const keyCodes = {
    UNDEFINED: 0,
    BACKSPACE: 8,
    TAB: 9,
    CLEAR: 12,
    ENTER: 13,
    SHIFT: 16,
    CTRL: 17,
    ALT: 18,
    BREAK: 19,
    CAPSLOCK: 20,
    ESCAPE: 27,
    SPACE: 32,
    PAGEUP: 33,
    PAGEDOWN: 34,
    END: 35,
    HOME: 36,
    LEFT: 37,
    UP: 38,
    RIGHT: 39,
    DOWN: 40,
    PRINTSCREEN: 44,
    INSERT: 45,
    DELETE: 46,
    K0: 48,
    K1: 49,
    K2: 50,
    K3: 51,
    K4: 52,
    K5: 53,
    K6: 54,
    K7: 55,
    K8: 56,
    K9: 57,
    A: 65,
    B: 66,
    C: 67,
    D: 68,
    E: 69,
    F: 70,
    G: 71,
    H: 72,
    I: 73,
    J: 74,
    K: 75,
    L: 76,
    M: 77,
    N: 78,
    O: 79,
    P: 80,
    Q: 81,
    R: 82,
    S: 83,
    T: 84,
    U: 85,
    V: 86,
    W: 87,
    X: 88,
    Y: 89,
    Z: 90,
    /* also META on Mac */
    WINDOWS: 91,
    MENU: 93,
    NUMPAD0: 96,
    NUMPAD1: 97,
    NUMPAD2: 98,
    NUMPAD3: 99,
    NUMPAD4: 100,
    NUMPAD5: 101,
    NUMPAD6: 102,
    NUMPAD7: 103,
    NUMPAD8: 104,
    NUMPAD9: 105,
    NUMPADASTERISK: 106,
    NUMPADPLUS: 107,
    /* on numeric keypad in eg Germany*/
    NUMPAD_DECIMAL_COMMA: 108,
    NUMPADMINUS: 109,
    /* on numeric keypad */
    NUMPAD_DECIMAL_POINT: 110,
    NUMPADSLASH: 111,
    F1: 112,
    F2: 113,
    F3: 114,
    F4: 115,
    F5: 116,
    F6: 117,
    F7: 118,
    F8: 119,
    F9: 120,
    F10: 121,
    F11: 122,
    F12: 123,
    NUMLOCK: 144,
    SCROLL_LOCK: 145,
    VOLUMEUP: 174,
    VOLUMEDOWN: 175,
    FASTFORWARD: 176,
    FASTREWIND: 177,
    PLAYPAUSE: 179,
    COMMA: 188,
    PERIOD: 190,
    SLASH: 191,
    LEFT_SQUARE_BRACKET: 219,
    BACKSLASH: 220,
    RIGHT_SQUARE_BRACKET: 221,
    NUMPADENTER: 255, // hack, jsbeeb only
    SHIFT_LEFT: 256, // hack, jsbeeb only
    SHIFT_RIGHT: 257, // hack, jsbeeb only
    ALT_LEFT: 258, // hack, jsbeeb only
    ALT_RIGHT: 259, // hack, jsbeeb only
    CTRL_LEFT: 260, // hack, jsbeeb only
    CTRL_RIGHT: 261, // hack, jsbeeb only
};

function detectKeyboardLayout() {
    if (localStorage.keyboardLayout) {
        return localStorage.keyboardLayout === "US" ? "US" : "UK";
    }
    if (navigator.language) {
        if (navigator.language.toLowerCase() === "en-gb") return "UK";
        if (navigator.language.toLowerCase() === "en-us") return "US";
    }
    return "UK"; // Default guess of UK
}

const isUKlayout = detectKeyboardLayout() === "UK";

if (isFirefox()) {
    keyCodes.SEMICOLON = 59;
    // #~ key (not on US keyboard)
    keyCodes.HASH = 163;
    keyCodes.APOSTROPHE = 222;
    keyCodes.BACK_QUOTE = 192;
    // Firefox doesn't return a keycode for this
    keyCodes.MUTE = -1;
    keyCodes.MINUS = 173;
    keyCodes.EQUALS = 61;
} else {
    // Chrome
    // TODO: check other browsers
    // https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent.keyCode
    keyCodes.SEMICOLON = 186;
    // #~ key (not on US keyboard)
    keyCodes.HASH = isUKlayout ? 222 : 223;
    keyCodes.APOSTROPHE = isUKlayout ? 192 : 222;
    keyCodes.MUTE = 173;
    keyCodes.MINUS = 189;
    keyCodes.EQUALS = 187;
    keyCodes.BACK_QUOTE = isUKlayout ? 223 : 192;
}

// Swap APOSTROPHE and BACK_QUOTE keys around for Mac users.  They are the opposite to what jsbeeb expects.
// Swap them to what jsbeeb expects, and tidy up the hash key to prevent duplicate key mappings.
if (window.navigator.userAgent.indexOf("Mac") !== -1) {
    keyCodes.BACK_QUOTE = 192;
    keyCodes.APOSTROPHE = 222;
    keyCodes.HASH = 223;
}

export function getKeyMap(keyLayout) {
    const keys2 = [];

    // shift pressed
    keys2[true] = {};

    // shift not pressed
    keys2[false] = {};

    // shiftDown MUST be true or false (not undefined)
    function doMap(s, colRow, shiftDown) {
        if (keys2[shiftDown][s] && keys2[shiftDown][s] !== colRow) {
            console.log(
                "Warning: duplicate binding for key",
                (shiftDown ? "<SHIFT>" : "") + s,
                colRow,
                keys2[shiftDown][s]
            );
        }
        keys2[shiftDown][s] = colRow;
    }

    // shiftDown undefined -> map both
    function map(s, colRow, shiftDown) {
        if ((!s && s !== 0) || !colRow) {
            console.log("error binding key", s, colRow);
        }
        if (typeof s === "string") {
            s = s.charCodeAt(0);
        }

        if (shiftDown === undefined) {
            doMap(s, colRow, true);
            doMap(s, colRow, false);
        } else {
            doMap(s, colRow, shiftDown);
        }
    }

    map(keyCodes.Q, BBC.Q);
    map(keyCodes.W, BBC.W);
    map(keyCodes.E, BBC.E);
    map(keyCodes.R, BBC.R);
    map(keyCodes.T, BBC.T);
    map(keyCodes.Y, BBC.Y);
    map(keyCodes.U, BBC.U);
    map(keyCodes.I, BBC.I);
    map(keyCodes.O, BBC.O);
    map(keyCodes.P, BBC.P);

    map(keyCodes.A, BBC.A);
    map(keyCodes.S, BBC.S);
    map(keyCodes.D, BBC.D);
    map(keyCodes.F, BBC.F);
    map(keyCodes.G, BBC.G);
    map(keyCodes.H, BBC.H);
    map(keyCodes.J, BBC.J);
    map(keyCodes.K, BBC.K);
    map(keyCodes.L, BBC.L);

    map(keyCodes.Z, BBC.Z);
    map(keyCodes.X, BBC.X);
    map(keyCodes.C, BBC.C);
    map(keyCodes.V, BBC.V);
    map(keyCodes.B, BBC.B);
    map(keyCodes.N, BBC.N);
    map(keyCodes.M, BBC.M);

    map(keyCodes.F10, BBC.F0); // F0 (mapped to F10)
    map(keyCodes.F1, BBC.F1);
    map(keyCodes.F2, BBC.F2);
    map(keyCodes.F3, BBC.F3);
    map(keyCodes.F4, BBC.F4);
    map(keyCodes.F5, BBC.F5);
    map(keyCodes.F6, BBC.F6);
    map(keyCodes.F7, BBC.F7);
    map(keyCodes.F8, BBC.F8);
    map(keyCodes.F9, BBC.F9);

    // these keys are in the same place on PC and BBC keyboards
    // including shifted characters
    // so can be the same for "natural" and "gaming"
    map(keyCodes.COMMA, BBC.COMMA);
    map(keyCodes.PERIOD, BBC.PERIOD);
    map(keyCodes.SLASH, BBC.SLASH);
    map(keyCodes.SPACE, BBC.SPACE);
    map(keyCodes.TAB, BBC.TAB);
    map(keyCodes.ENTER, BBC.RETURN);

    map(keyCodes.SHIFT, BBC.SHIFT);
    // see later map(keyCodes.SHIFT_LEFT, BBC.SHIFT_LEFT);
    map(keyCodes.SHIFT_RIGHT, BBC.SHIFT);

    // other keys to map to these in "game" layout too
    map(keyCodes.LEFT, BBC.LEFT);
    map(keyCodes.UP, BBC.UP);
    map(keyCodes.RIGHT, BBC.RIGHT);
    map(keyCodes.DOWN, BBC.DOWN);

    // Physical, and default
    map(keyCodes.K1, BBC.K1);
    map(keyCodes.K2, BBC.K2);
    map(keyCodes.K3, BBC.K3);
    map(keyCodes.K4, BBC.K4);
    map(keyCodes.K5, BBC.K5);
    map(keyCodes.K6, BBC.K6);
    map(keyCodes.K7, BBC.K7);
    map(keyCodes.K8, BBC.K8);
    map(keyCodes.K9, BBC.K9);
    map(keyCodes.K0, BBC.K0);
    map(keyCodes.SHIFT_LEFT, BBC.SHIFT);
    map(keyCodes.EQUALS, BBC.HAT_TILDE); // ^~ on +/=
    map(keyCodes.SEMICOLON, BBC.SEMICOLON_PLUS); // ';' / '+'
    map(keyCodes.MINUS, BBC.MINUS); // '-' / '=' mapped to underscore
    map(keyCodes.LEFT_SQUARE_BRACKET, BBC.LEFT_SQUARE_BRACKET); // maps to [{
    map(keyCodes.RIGHT_SQUARE_BRACKET, BBC.RIGHT_SQUARE_BRACKET); // maps to ]}
    map(keyCodes.COMMA, BBC.COMMA); // ',' / '<'
    map(keyCodes.PERIOD, BBC.PERIOD); // '.' / '>'
    map(keyCodes.SLASH, BBC.SLASH); // '/' / '?'
    map(keyCodes.WINDOWS, BBC.SHIFTLOCK); // shift lock mapped to "windows" key
    map(keyCodes.TAB, BBC.TAB); // tab
    map(keyCodes.ENTER, BBC.RETURN); // return
    map(keyCodes.DELETE, BBC.DELETE); // delete
    map(keyCodes.BACKSPACE, BBC.DELETE); // delete
    map(keyCodes.END, BBC.COPY); // copy key is end
    map(keyCodes.F11, BBC.COPY); // copy key is end for Apple
    map(keyCodes.SHIFT, BBC.SHIFT); // shift
    map(keyCodes.ESCAPE, BBC.ESCAPE); // escape
    map(keyCodes.CTRL, BBC.CTRL);
    map(keyCodes.CTRL_LEFT, BBC.CTRL);
    map(keyCodes.CTRL_RIGHT, BBC.CTRL);
    map(keyCodes.CAPSLOCK, BBC.CAPSLOCK); // caps (on Rich's/Mike's computer)
    map(keyCodes.LEFT, BBC.LEFT); // arrow left
    map(keyCodes.UP, BBC.UP); // arrow up
    map(keyCodes.RIGHT, BBC.RIGHT); // arrow right
    map(keyCodes.DOWN, BBC.DOWN); // arrow down
    map(keyCodes.APOSTROPHE, BBC.COLON_STAR);
    map(keyCodes.HASH, BBC.RIGHT_SQUARE_BRACKET);

    // None of this last group in great locations.
    // But better to have them mapped at least somewhere.
    map(keyCodes.BACK_QUOTE, BBC.AT);
    map(keyCodes.BACKSLASH, BBC.PIPE_BACKSLASH);
    map(keyCodes.PAGEUP, BBC.UNDERSCORE_POUND);

    return keys2;
}

export function hexbyte(value) {
    return ((value >>> 4) & 0xf).toString(16) + (value & 0xf).toString(16);
}

export function hexword(value) {
    return hexbyte(value >>> 8) + hexbyte(value & 0xff);
}

export function hd(reader, start, end, opts) {
    opts = opts || {};
    const width = opts.width || 16;
    const gap = opts.gap === undefined ? 8 : opts.gap;
    const res = [];
    let str = "";
    let j = 0;
    for (let i = start; i < end; ++i) {
        str += " ";
        str += hexbyte(reader(i));
        if (++j === gap) str += " ";
        if (j === width) {
            res.push(str);
            str = "";
            j = 0;
        }
    }
    if (str) res.push(str);
    let joined = "";
    for (let i = 0; i < res.length; ++i) {
        joined += hexword(start + i * width) + " :" + res[i] + "\n";
    }
    return joined;
}

const signExtendTable = (function () {
    const table = [];
    for (let i = 0; i < 256; ++i) table[i] = i >= 128 ? i - 256 : i;
    return table;
})();

export function signExtend(val) {
    return signExtendTable[val | 0] | 0;
}

export function noop() {}

export function bench() {
    for (let j = 0; j < 10; ++j) {
        let res = 0;
        const start = Date.now();
        for (let i = 0; i < 4096 * 1024; ++i) {
            res += signExtend(i & 0xff);
        }
        const tt = Date.now() - start;
        console.log(res, tt);
    }
}

export function noteEvent(category, type, label) {
    console.log("Event noted:", category);
}

let baseUrl = "";

export function setBaseUrl(url) {
    baseUrl = url;
}

export function uint8ArrayToString(array) {
    let str = "";
    for (let i = 0; i < array.length; ++i) str += String.fromCharCode(array[i]);
    return str;
}

export function stringToUint8Array(str) {
    if (str instanceof Uint8Array) return str;
    const len = str.length;
    const array = new Uint8Array(len);
    for (let i = 0; i < len; ++i) array[i] = str.charCodeAt(i) & 0xff;
    return array;
}

export function loadData(url) {
    return new Promise(function (resolve, reject) {
        const request = new XMLHttpRequest();
        request.open("GET", baseUrl + url, true);
        request.overrideMimeType("text/plain; charset=x-user-defined");
        request.onload = function () {
            if (request.status !== 200) reject(new Error("Unable to load " + url + ", http code " + request.status));
            if (typeof request.response !== "string") {
                resolve(request.response);
            } else {
                resolve(stringToUint8Array(request.response));
            }
        };
        request.onerror = function () {
            reject(new Error("A network error occurred loading " + url));
        };
        request.send(null);
    });
}

export function readInt32(data, offset) {
    return (data[offset + 3] << 24) | (data[offset + 2] << 16) | (data[offset + 1] << 8) | data[offset + 0];
}

export function readInt16(data, offset) {
    return (data[offset + 1] << 8) | data[offset + 0];
}

const tempBuf = new ArrayBuffer(4);
const tempBuf8 = new Uint8Array(tempBuf);
const tempBufF32 = new Float32Array(tempBuf);

export function readFloat32(data, offset) {
    tempBuf8[0] = data[offset];
    tempBuf8[1] = data[offset + 1];
    tempBuf8[2] = data[offset + 2];
    tempBuf8[3] = data[offset + 3];
    return tempBufF32[0];
}

export function makeFast32(u32) {
    // Firefox is ~5% faster with signed 32-bit arrays. Chrome is the same speed
    // either way, so here we unconditionally wrap all u32 buffers as i32.
    // Having a function do this makes it easy to test u32 vs i32, and means we
    // keep the rest of the code using u32 (which makes more sense to me).
    return new Int32Array(u32.buffer);
}

export function discImageSize(name) {
    // SSD, aka. single-sided disc, is:
    // - 1 side :)
    // - 80 tracks.
    // - 10 sectors per track.
    // - 256 bytes per sector.
    let isDsd = false;
    let isDoubleDensity = false;
    let byteSize = 80 * 10 * 256;
    // DSD, aka. double-sided disc is twice the size.
    let lowerName = name.toLowerCase();
    if (lowerName.endsWith(".dsd")) {
        byteSize *= 2;
        isDsd = true;
    }
    if (lowerName.endsWith(".adl")) {
        // ADFS (Large) disks are:
        // double density, double sided, 80 track, 16 sectors per track, 256 bytes per sector (640K)
        byteSize = 2 * 80 * 16 * 256;
        isDsd = true;
        isDoubleDensity = true;
    }
    if (lowerName.endsWith(".adf") || lowerName.endsWith(".adm")) {
        // ADFS (Small) disks are:
        // single density, double sided, 80 track, 16 sectors per track, 256 bytes per sector (640K)
        byteSize = 80 * 16 * 256;
        isDsd = false;
        isDoubleDensity = true;
    }
    return { isDsd: isDsd, isDoubleDensity: isDoubleDensity, byteSize: byteSize };
}

export function setDiscName(data, name) {
    for (let i = 0; i < 8; ++i) data[i] = name.charCodeAt(i) & 0xff;
}

export function resizeUint8Array(array, byteSize) {
    const newArray = new Uint8Array(byteSize);
    newArray.set(array.subarray(0, byteSize));
    return newArray;
}

export function Fifo(capacity) {
    this.buffer = new Uint8Array(capacity);
    this.size = 0;
    this.wPtr = 0;
    this.rPtr = 0;
}

Fifo.prototype.full = function () {
    return this.size === this.buffer.length;
};

Fifo.prototype.empty = function () {
    return this.size === 0;
};

Fifo.prototype.clear = function () {
    this.size = 0;
    this.wPtr = 0;
    this.rPtr = 0;
};

Fifo.prototype.put = function (b) {
    if (this.full()) return;
    this.buffer[this.wPtr % this.buffer.length] = b;
    this.wPtr++;
    this.size++;
};

Fifo.prototype.get = function () {
    if (this.empty()) return;
    const res = this.buffer[this.rPtr % this.buffer.length];
    this.rPtr++;
    this.size--;
    return res;
};
