import * as utils from "./utils.js";
import { FakeVideo, Video } from "./video.js";
import { Debugger } from "./web/debug.js";
import { Cpu6502 } from "./6502.js";
import { Cmos } from "./cmos.js";
import * as disc from "./fdc.js";
import { starCat } from "./discs/cat.js";
import * as canvasLib from "./canvas.js";
import { Config } from "./config.js";
import { AudioHandler } from "./web/audio-handler.js";
import { Econet } from "./econet.js";
import { jrvideo } from "./js/video.js";

let processor;
let video;
let dbgr;
let frames = 0;
let frameSkip = 0;
let syncLights;
let running;
let model;
let availableImages;
let discImage;
let secondDiscImage;
const extraRoms = [];
if (typeof starCat === "function") {
    availableImages = starCat();

    if (availableImages && availableImages[0] && availableImages[1]) {
        discImage = availableImages[0].file;
        secondDiscImage = availableImages[1].file;
    }
}
let queryString = document.location.search.substring(1) + "&" + window.location.hash.substring(1);
let parsedQuery = {};
let needsAutoboot = false;
let autoType = "";
let keyLayout = window.localStorage.keyLayout || "physical";

const BBC = utils.BBC;
const keyCodes = utils.keyCodes;
const emuKeyHandlers = {};
let cpuMultiplier = 1;
let fastAsPossible = false;
let fastTape = false;
let noSeek = false;
let pauseEmu = false;
let stepEmuWhenPaused = false;
let audioFilterFreq = 7000;
let audioFilterQ = 5;
let stationId = 101;

if (queryString) {
    if (queryString[queryString.length - 1] === "/")
        // workaround for shonky python web server
        queryString = queryString.substring(0, queryString.length - 1);
    queryString.split("&").forEach(function (keyval) {
        const keyAndVal = keyval.split("=");
        const key = decodeURIComponent(keyAndVal[0]);
        let val = null;
        if (keyAndVal.length > 1) val = decodeURIComponent(keyAndVal[1]);
        parsedQuery[key] = val;

        // eg KEY.CAPSLOCK=CTRL
        let bbcKey;
        if (key.toUpperCase().indexOf("KEY.") === 0) {
            bbcKey = val.toUpperCase();

            if (BBC[bbcKey]) {
                const nativeKey = key.substring(4).toUpperCase(); // remove KEY.
                if (keyCodes[nativeKey]) {
                    console.log("mapping " + nativeKey + " to " + bbcKey);
                    utils.userKeymap.push({ native: nativeKey, bbc: bbcKey });
                } else {
                    console.log("unknown key: " + nativeKey);
                }
            } else {
                console.log("unknown BBC key: " + val);
            }
        } else {
            switch (key) {
                case "LEFT":
                case "RIGHT":
                case "UP":
                case "DOWN":
                case "autoboot":
                    needsAutoboot = "boot";
                    break;
                case "autochain":
                    needsAutoboot = "chain";
                    break;
                case "autorun":
                    needsAutoboot = "run";
                    break;
                case "autotype":
                    needsAutoboot = "type";
                    autoType = val;
                    break;
                case "keyLayout":
                    keyLayout = (val + "").toLowerCase();
                    break;
                case "disc":
                case "disc1":
                    discImage = val;
                    break;
                case "rom":
                    extraRoms.push(val);
                    break;
                case "disc2":
                    secondDiscImage = val;
                    break;
                case "embed":
                    $(".embed-hide").hide();
                    $("body").css("background-color", "transparent");
                    break;
                case "fasttape":
                    fastTape = true;
                    break;
                case "noseek":
                    noSeek = true;
                    break;
                case "audiofilterfreq":
                    audioFilterFreq = Number(val);
                    break;
                case "audiofilterq":
                    audioFilterQ = Number(val);
                    break;
                case "stationId":
                    stationId = Number(val);
                    break;
            }
        }
    });
}

if (parsedQuery.frameSkip) frameSkip = parseInt(parsedQuery.frameSkip);

const emulationConfig = {
    keyLayout: keyLayout,
    coProcessor: parsedQuery.coProcessor,
    cpuMultiplier: cpuMultiplier,
    videoCyclesBatch: parsedQuery.videoCyclesBatch,
    extraRoms: extraRoms,
};

const config = new Config();
config.setModel(parsedQuery.model || guessModelFromUrl());
config.setKeyLayout(keyLayout);
config.set65c02(parsedQuery.coProcessor);
config.setEconet(true);
config.setMusic5000(true);
config.setTeletext(true);

model = config.model;

if (parsedQuery.cpuMultiplier) {
    cpuMultiplier = parseFloat(parsedQuery.cpuMultiplier);
    console.log("CPU multiplier set to " + cpuMultiplier);
}
const clocksPerSecond = (cpuMultiplier * 2 * 1000 * 1000) | 0;
const MaxCyclesPerFrame = clocksPerSecond / 10;

var screen = new jrvideo();

let tryGl = false;
if (parsedQuery.glEnabled !== undefined) {
    tryGl = parsedQuery.glEnabled === "true";
}
const $screen = $("#canvas");
const canvas = tryGl ? canvasLib.bestCanvas($screen[0]) : new canvasLib.Canvas($screen[0]);
video = new Video(model.isMaster, canvas.fb32, function paint(minx, miny, maxx, maxy) {
    frames++;
    if (frames < frameSkip) return;
    frames = 0;
//    canvas.paint(minx, miny, maxx, maxy);
});
if (parsedQuery.fakeVideo !== undefined) video = new FakeVideo();

const audioHandler = new AudioHandler($("#audio-warning"), audioFilterFreq, audioFilterQ, noSeek);

let lastShiftLocation = 1;
let lastCtrlLocation = 1;
let lastAltLocation = 1;

dbgr = new Debugger(video);

$(".initially-hidden").removeClass("initially-hidden");

function keyCode(evt) {
    const ret = evt.which || evt.charCode || evt.keyCode;

    const keyCodes = utils.keyCodes;

    switch (evt.location) {
        default:
            // keyUp events seem to pass location = 0 (Chrome)
            switch (ret) {
                case keyCodes.SHIFT:
                    if (lastShiftLocation === 1) {
                        return keyCodes.SHIFT_LEFT;
                    } else {
                        return keyCodes.SHIFT_RIGHT;
                    }

                case keyCodes.ALT:
                    if (lastAltLocation === 1) {
                        return keyCodes.ALT_LEFT;
                    } else {
                        return keyCodes.ALT_RIGHT;
                    }

                case keyCodes.CTRL:
                    if (lastCtrlLocation === 1) {
                        return keyCodes.CTRL_LEFT;
                    } else {
                        return keyCodes.CTRL_RIGHT;
                    }
            }
            break;
        case 1:
            switch (ret) {
                case keyCodes.SHIFT:
                    lastShiftLocation = 1;
                    return keyCodes.SHIFT_LEFT;

                case keyCodes.ALT:
                    lastAltLocation = 1;
                    return keyCodes.ALT_LEFT;

                case keyCodes.CTRL:
                    lastCtrlLocation = 1;
                    return keyCodes.CTRL_LEFT;
            }
            break;
        case 2:
            switch (ret) {
                case keyCodes.SHIFT:
                    lastShiftLocation = 2;
                    return keyCodes.SHIFT_RIGHT;

                case keyCodes.ALT:
                    lastAltLocation = 2;
                    return keyCodes.ALT_RIGHT;

                case keyCodes.CTRL:
                    lastCtrlLocation = 2;
                    return keyCodes.CTRL_RIGHT;
            }
            break;
        case 3: // numpad
            switch (ret) {
                case keyCodes.ENTER:
                    return utils.keyCodes.NUMPADENTER;

                case keyCodes.DELETE:
                    return utils.keyCodes.NUMPAD_DECIMAL_POINT;
            }
            break;
    }

    return ret;
}

function keyPress(evt) {
    if (document.activeElement.id === "paste-text") return;
    if (running || (!dbgr.enabled() && !pauseEmu)) return;
    const code = keyCode(evt);
    if (dbgr.enabled() && code === 103 /* lower case g */) {
        dbgr.hide();
        go();
        return;
    }
    if (pauseEmu) {
        if (code === 103 /* lower case g */) {
            pauseEmu = false;
            go();
            return;
        } else if (code === 110 /* lower case n */) {
            stepEmuWhenPaused = true;
            go();
            return;
        }
    }
    const handled = dbgr.keyPress(keyCode(evt));
    if (handled) evt.preventDefault();
}

emuKeyHandlers[utils.keyCodes.S] = function (down) {
    if (down) {
        utils.noteEvent("keyboard", "press", "S");
        stop(true);
    }
};
emuKeyHandlers[utils.keyCodes.R] = function (down) {
    if (down) window.location.reload();
};

function keyDown(evt) {
    audioHandler.tryResume();
    if (document.activeElement.id === "paste-text") return;
    if (!running) return;
    const code = keyCode(evt);
    if (evt.altKey) {
        const handler = emuKeyHandlers[code];
        if (handler) {
            handler(true, code);
            evt.preventDefault();
        }
    } else if (code === utils.keyCodes.HOME && evt.ctrlKey) {
        utils.noteEvent("keyboard", "press", "home");
        stop(true);
    } else if (code === utils.keyCodes.INSERT && evt.ctrlKey) {
        utils.noteEvent("keyboard", "press", "insert");
        fastAsPossible = !fastAsPossible;
    } else if (code === utils.keyCodes.END && evt.ctrlKey) {
        utils.noteEvent("keyboard", "press", "end");
        pauseEmu = true;
        stop(false);
    } else if (code === utils.keyCodes.F12 || code === utils.keyCodes.BREAK) {
        utils.noteEvent("keyboard", "press", "break");
        processor.setReset(true);
        evt.preventDefault();
    } else if (code === utils.keyCodes.B && evt.ctrlKey) {
        // Ctrl-B turns on the printer, so we open a printer output
        // window in addition to passing the keypress along to the beeb.
        processor.sysvia.keyDown(keyCode(evt), evt.shiftKey);
        evt.preventDefault();
    } else {
        processor.sysvia.keyDown(keyCode(evt), evt.shiftKey);
        evt.preventDefault();
    }
}

function keyUp(evt) {
    if (document.activeElement.id === "paste-text") return;
    // Always let the key ups come through. That way we don't cause sticky keys in the debugger.
    const code = keyCode(evt);
    if (processor && processor.sysvia) processor.sysvia.keyUp(code);
    if (!running) return;
    if (evt.altKey) {
        const handler = emuKeyHandlers[code];
        if (handler) {
            handler(false, code);
            evt.preventDefault();
        }
    } else if (code === utils.keyCodes.F12 || code === utils.keyCodes.BREAK) {
        processor.setReset(false);
    }
    evt.preventDefault();
}

function loadHTMLFile(file) {
    const reader = new FileReader();
    reader.onload = function (e) {
        if ($('input[name="driveSelector"]:checked').val() === "0") {
            processor.fdc.loadDisc(0, disc.discFor(processor.fdc, file.name, e.target.result));
            delete parsedQuery.disc;
            delete parsedQuery.disc1;
            updateUrl();
            $discsModal.hide();
        } else {
            processor.fdc.loadDisc(1, disc.discFor(processor.fdc, file.name, e.target.result));
            delete parsedQuery.disc2;
            updateUrl();
            $discsModal.hide();
        }
    };
    reader.readAsBinaryString(file);
}

function setDiscImage(drive, name) {
    delete parsedQuery.disc;
    if (drive === 0) {
        parsedQuery.disc1 = name;
    } else {
        parsedQuery.disc2 = name;
    }
    updateUrl();
}

function loadSCSIFile(file) {
    const reader = new FileReader();
    reader.onload = function (e) {
        processor.filestore.scsi = utils.stringToUint8Array(e.target.result);

        processor.filestore.PC = 0x400;
        processor.filestore.SP = 0xff;
        processor.filestore.A = 1;
        processor.filestore.emulationSpeed = 0;

        // Reset any open receive blocks
        processor.econet.receiveBlocks = [];
        processor.econet.nextReceiveBlockNumber = 1;

        $fsModal.hide();
    };
    reader.readAsBinaryString(file);
}

const $cub = $("#cub-monitor");
$cub.on("mousemove mousedown mouseup", function (evt) {
    audioHandler.tryResume();
    if (document.activeElement !== document.body) document.activeElement.blur();
    const cubOffset = $cub.offset();
    const screenOffset = $screen.offset();
    const x = (evt.offsetX - cubOffset.left + screenOffset.left) / $screen.width();
    const y = (evt.offsetY - cubOffset.top + screenOffset.top) / $screen.height();
    if (processor.touchScreen) processor.touchScreen.onMouse(x, y, evt.buttons);
    evt.preventDefault();
});

$(window).blur(function () {
    if (processor.sysvia) processor.sysvia.clearKeys();
});

$("#fs").click(function (event) {
    $screen[0].requestFullscreen();
    event.preventDefault();
});

document.onkeydown = keyDown;
document.onkeypress = keyPress;
document.onkeyup = keyUp;

window.onbeforeunload = function () {
    if (running && processor.sysvia.hasAnyKeyDown()) {
        return (
            "It seems like you're still using the emulator. If you're in Chrome, it's impossible for jsbeeb to prevent some shortcuts (like ctrl-W) from performing their default behaviour (e.g. closing the window).\n" +
            "As a workarond, create an 'Application Shortcut' from the Tools menu.  When jsbeeb runs as an application, it *can* prevent ctrl-W from closing the window."
        );
    }
};

const econet = new Econet(stationId);
const cmos = new Cmos(
    {
        load: function () {
            if (window.localStorage.cmosRam) {
                return JSON.parse(window.localStorage.cmosRam);
            }
            return null;
        },
        save: function (data) {
            window.localStorage.cmosRam = JSON.stringify(data);
        },
    },
    model.cmosOverride,
    econet
);

processor = new Cpu6502(
    model,
    dbgr,
    video,
    audioHandler.soundChip,
    model.hasMusic5000 ? audioHandler.music5000 : null,
    cmos,
    emulationConfig,
    econet
);

function sendRawKeyboardToBBC(keysToSend, checkCapsAndShiftLocks) {
    let lastChar;
    let nextKeyMillis = 0;
    processor.sysvia.disableKeyboard();

    if (checkCapsAndShiftLocks) {
        let toggleKey = null;
        if (!processor.sysvia.capsLockLight) toggleKey = BBC.CAPSLOCK;
        else if (processor.sysvia.shiftLockLight) toggleKey = BBC.SHIFTLOCK;
        if (toggleKey) {
            keysToSend.unshift(toggleKey);
            keysToSend.push(toggleKey);
        }
    }

    const sendCharHook = processor.debugInstruction.add(function nextCharHook() {
        const millis = processor.cycleSeconds * 1000 + processor.currentCycles / (clocksPerSecond / 1000);
        if (millis < nextKeyMillis) {
            return;
        }

        if (lastChar && lastChar !== utils.BBC.SHIFT) {
            processor.sysvia.keyToggleRaw(lastChar);
        }

        if (keysToSend.length === 0) {
            // Finished
            processor.sysvia.enableKeyboard();
            sendCharHook.remove();
            return;
        }

        const ch = keysToSend[0];
        const debounce = lastChar === ch;
        lastChar = ch;
        if (debounce) {
            lastChar = undefined;
            nextKeyMillis = millis + 30;
            return;
        }

        let time = 50;
        if (typeof lastChar === "number") {
            time = lastChar;
            lastChar = undefined;
        } else {
            processor.sysvia.keyToggleRaw(lastChar);
        }

        // remove first character
        keysToSend.shift();

        nextKeyMillis = millis + time;
    });
}

function autoboot(image) {
    const BBC = utils.BBC;

    console.log("Autobooting disc");
    utils.noteEvent("init", "autoboot", image);

    // Shift-break simulation, hold SHIFT for 1000ms.
    sendRawKeyboardToBBC([BBC.SHIFT, 1000], false);
}

function autoBootType(keys) {
    console.log("Auto typing '" + keys + "'");
    utils.noteEvent("init", "autochain");

    const bbcKeys = utils.stringToBBCKeys(keys);
    sendRawKeyboardToBBC([1000].concat(bbcKeys), false);
}

function updateUrl() {
    let url = window.location.origin + window.location.pathname;
    let sep = "?";
    $.each(parsedQuery, function (key, value) {
        if (key.length > 0 && value) {
            url += sep + encodeURIComponent(key) + "=" + encodeURIComponent(value);
            sep = "&";
        }
    });
    window.history.pushState(null, null, url);
}

const $errorDialog = $("#error-dialog");

function showError(context, error) {
    $errorDialog.find(".context").text(context);
    $errorDialog.find(".error").text(error);
    $errorDialogModal.show();
}

function splitImage(image) {
    const match = image.match(/(([^:]+):\/?\/?|[!^|])?(.*)/);
    const schema = match[2] || match[1] || "";
    image = match[3];
    return { image: image, schema: schema };
}

function loadDiscImage(discImage) {
    if (!discImage) return Promise.resolve(null);
    const split = splitImage(discImage);
    discImage = split.image;
    const schema = split.schema;
    if (schema[0] === "!" || schema === "local") {
        return Promise.resolve(disc.localDisc(processor.fdc, discImage));
    }
    // TODO: come up with a decent UX for passing an 'onChange' parameter to each of these.
    // Consider:
    // * hashing contents and making a local disc image named by original disc hash, save by that, and offer
    //   to load the modified disc on load.
    // * popping up a message that notes the disc has changed, and offers a way to make a local image
    // * Dialog box (ugh) saying "is this ok?"
    if (schema === "b64data") {
        const ssdData = atob(discImage);
        discImage = "disk.ssd";
        return Promise.resolve(disc.discFor(processor.fdc, discImage, ssdData));
    }
    if (schema === "data") {
        const arr = Array.prototype.map.call(atob(discImage), (x) => x.charCodeAt(0));
        const unzipped = utils.unzipDiscImage(arr);
        const discData = unzipped.data;
        discImage = unzipped.name;
        return Promise.resolve(disc.discFor(processor.fdc, discImage, discData));
    }
    if (schema === "http" || schema === "https" || schema === "file") {
        return utils.loadData(schema + "://" + discImage).then(function (discData) {
            if (/\.zip/i.test(discImage)) {
                const unzipped = utils.unzipDiscImage(discData);
                discData = unzipped.data;
                discImage = unzipped.name;
            }
            return disc.discFor(processor.fdc, discImage, discData);
        });
    }

    return disc.load("discs/" + discImage).then(function (discData) {
        return disc.discFor(processor.fdc, discImage, discData);
    });
}

$("#disc_load").on("change", function (evt) {
    if (evt.target.files.length === 0) return;
    utils.noteEvent("local", "click"); // NB no filename here
    const file = evt.target.files[0];
    loadHTMLFile(file);
    evt.target.value = ""; // clear so if the user picks the same file again after a reset we get a "change"
});

$("#fs_load").on("change", function (evt) {
    if (evt.target.files.length === 0) return;
    utils.noteEvent("local", "click"); // NB no filename here
    const file = evt.target.files[0];
    loadSCSIFile(file);
    evt.target.value = ""; // clear so if the user picks the same file again after a reset we get a "change"
});

function anyModalsVisible() {
    return $(".modal:visible").length !== 0;
}

let modalSavedRunning = false;
document.addEventListener("show.bs.modal", function () {
    if (!anyModalsVisible()) modalSavedRunning = running;
    if (running) stop(false);
});
document.addEventListener("hidden.bs.modal", function () {
    if (!anyModalsVisible() && modalSavedRunning) {
        go();
    }
});

const discList = $("#disc-list");
const template = discList.find(".template");
$.each(availableImages, function (i, image) {
    const elem = template.clone().removeClass("template").appendTo(discList);
    elem.find(".name").text(image.name);
    $(elem).on("click", function () {
        utils.noteEvent("images", "click", image.file);

        if ($('input[name="driveSelector"]:checked').val() === "0") {
            setDiscImage(0, image.file);
            loadDiscImage(parsedQuery.disc1).then(function (disc) {
                processor.fdc.loadDisc(0, disc);
            });
        } else {
            setDiscImage(1, image.file);
            loadDiscImage(parsedQuery.disc2).then(function (disc) {
                processor.fdc.loadDisc(1, disc);
            });
        }

        $discsModal.hide();
    });
});

$("#download-drive-link").on("click", function () {
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.style = "display: none";

    const blob = new Blob([processor.fdc.drives[$('input[name="driveSelector"]:checked').val()].data], {
            type: "application/octet-stream",
        }),
        url = window.URL.createObjectURL(blob);
    a.href = url;
    a.download = processor.fdc.drives[$('input[name="driveSelector"]:checked').val()].name;
    a.click();
    window.URL.revokeObjectURL(url);
});

$("#download-filestore-link").on("click", function () {
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.style = "display: none";

    const blob = new Blob([processor.filestore.scsi], { type: "application/octet-stream" }),
        url = window.URL.createObjectURL(blob);
    a.href = url;
    a.download = "scsi.dat";
    a.click();
    window.URL.revokeObjectURL(url);
});

$("#hard-reset").click(function (event) {
    processor.reset(true);
    event.preventDefault();
});

$("#soft-reset").click(function (event) {
    processor.reset(false);
    event.preventDefault();
});

function guessModelFromUrl() {
    if (window.location.hostname.indexOf("bbc") === 0) return "B-DFS1.2";
    if (window.location.hostname.indexOf("master") === 0) return "Master";
    return "B-DFS1.2";
}

function Light(name) {
    const dom = $("#" + name);
    let on = false;
    this.update = function (val) {
        if (val === on) return;
        on = val;
        dom.toggleClass("on", on);
    };
}

const caps = new Light("capslight");
const shift = new Light("shiftlight");
const floppy0 = new Light("floppy0");
const floppy1 = new Light("floppy1");
const network = new Light("networklight");

syncLights = function () {
    caps.update(processor.sysvia.capsLockLight);
    shift.update(processor.sysvia.shiftLockLight);
    floppy0.update(processor.fdc.motorOn[0]);
    floppy1.update(processor.fdc.motorOn[1]);
    network.update(processor.econet.activityLight());
};

const startPromise = Promise.all([audioHandler.initialise(), processor.initialise()]).then(function () {
    // Ideally would start the loads first. But their completion needs the FDC from the processor
    const imageLoads = [];
    if (discImage)
        imageLoads.push(
            loadDiscImage(discImage).then(function (disc) {
                processor.fdc.loadDisc(0, disc);
            })
        );
    if (secondDiscImage)
        imageLoads.push(
            loadDiscImage(secondDiscImage).then(function (disc) {
                processor.fdc.loadDisc(1, disc);
            })
        );

    return Promise.all(imageLoads);
});

startPromise.then(
    function () {
        switch (needsAutoboot) {
            case "boot":
                autoboot(discImage);
                break;
            case "type":
                autoBootType(autoType);
                break;
            default:
                break;
        }

        if (parsedQuery.patch) {
            dbgr.setPatch(parsedQuery.patch);
        }

        go();
    },
    function (error) {
        showError("initialising", error);
        console.log(error);
    }
);

let last = 0;

function VirtualSpeedUpdater() {
    this.cycles = 0;
    this.time = 0;
    this.v = $(".virtualMHz");
    this.header = $("#virtual-mhz-header");
    this.speedy = false;

    this.update = function (cycles, time, speedy) {
        this.cycles += cycles;
        this.time += time;
        this.speedy = speedy;
    };

    this.display = function () {
        // MRG would be nice to graph instantaneous speed to get some idea where the time goes.
        if (this.cycles) {
            const thisMHz = this.cycles / this.time / 1000;
            this.v.text(thisMHz.toFixed(1));
            if (this.cycles >= 10 * 2 * 1000 * 1000) {
                this.cycles = this.time = 0;
            }
            this.header.css("color", this.speedy ? "red" : "white");
        }
        setTimeout(this.display.bind(this), 3333);
    };

    this.display();
}

const virtualSpeedUpdater = new VirtualSpeedUpdater();

function draw(now) {
    if (!running) {
        last = 0;
        return;
    }
    // If we got here via setTimeout, we don't get passed the time.
    if (now === undefined) {
        now = window.performance.now();
    }

    const motorOn = processor.acia.motorOn;
    const discOn = processor.fdc.motorOn[0] || processor.fdc.motorOn[1];
    const speedy = fastAsPossible || (fastTape && motorOn);
    const useTimeout = speedy || motorOn || discOn;
    const timeout = speedy ? 0 : 1000.0 / 50;

    // In speedy mode, we still run all the state machines accurately
    // but we paint less often because painting is the most expensive
    // part of jsbeeb at this time.
    // We need need to paint per odd number of frames so that interlace
    // modes, i.e. MODE 7, still look ok.
    const frameSkipCount = speedy ? 9 : 0;
    video.frameSkipCount = frameSkipCount;

    // We use setTimeout instead of requestAnimationFrame in two cases:
    // a) We're trying to run as fast as possible.
    // b) Tape is playing, normal speed but backgrounded tab should run.
    if (useTimeout) {
        window.setTimeout(draw, timeout);
    } else {
        window.requestAnimationFrame(draw);
    }

    syncLights();
    if (last !== 0) {
        let cycles;
        if (!speedy) {
            // Now and last are DOMHighResTimeStamp, just a double.
            const sinceLast = now - last;
            cycles = (sinceLast * clocksPerSecond) / 1000;
            cycles = Math.min(cycles, MaxCyclesPerFrame);
        } else {
            cycles = clocksPerSecond / 50;
        }
        cycles |= 0;
        try {
            if (!processor.execute(cycles)) {
                stop(true);
            }
            const end = performance.now();
            virtualSpeedUpdater.update(cycles, end - now, speedy);
        } catch (e) {
            running = false;
            utils.noteEvent("exception", "thrown", e.stack);
            dbgr.debug(processor.pc);
            throw e;
        }
        if (stepEmuWhenPaused) {
            stop(false);
            stepEmuWhenPaused = false;
        }

        // jr update screen
        // Canvas
        const canvas = $("#bbcCanvas")[0];
        canvas.width = 480; 
        canvas.height = 500;
        
        var offset = ((processor.video.regs[12] * 256) + processor.video.regs[13]) - 0x2800;
        if(offset >= 0)
        {
            offset = 0x7c00 + offset;
            const ctx = canvas.getContext('2d');
            var imgData = ctx.createImageData(canvas.width, canvas.height);

            let pageBuffer = new Uint8Array(40 * 25);
            for(var i=0; i<40*25; i++)
            {
                if((offset + i) > 0x7fff) {
                    offset = 0x7c00 - i;
                }
                pageBuffer[i] = processor.readmem((offset + i));
            }

            screen.redraw(ctx, pageBuffer, imgData, 25);
        }
    }
    last = now;
}

function run() {
    window.requestAnimationFrame(draw);
}

let wasPreviouslyRunning = false;

function handleVisibilityChange() {
    if (document.visibilityState === "hidden") {
        wasPreviouslyRunning = running;
        const keepRunningWhenHidden = processor.acia.motorOn || processor.fdc.motorOn[0] || processor.fdc.motorOn[1];
        if (running && !keepRunningWhenHidden) {
            stop(false);
        }
    } else {
        if (wasPreviouslyRunning) {
            go();
        }
    }
}

document.addEventListener("visibilitychange", handleVisibilityChange, false);

function go() {
    audioHandler.unmute();
    running = true;
    run();
}

function stop(debug) {
    running = false;
    processor.stop();
    if (debug) dbgr.debug(processor.pc);
    audioHandler.mute();
};
