import * as utils from "./utils.js";
import { Video } from "./video.js";
import { Debugger } from "./web/debug.js";
import { Cpu6502 } from "./6502.js";
import * as disc from "./fdc.js";
import { starCat } from "./discs/cat.js";
import { Config } from "./config.js";
import { AudioHandler } from "./web/audio-handler.js";
import { jrvideo } from "./js/video.js";

let processor;
let video;
let dbgr;
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
let selectedDrive = 0;

if (queryString) {
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
config.setTeletext(true);
config.setMusic5000(true);

model = config.model;

if (parsedQuery.cpuMultiplier) {
    cpuMultiplier = parseFloat(parsedQuery.cpuMultiplier);
    console.log("CPU multiplier set to " + cpuMultiplier);
}
const clocksPerSecond = (cpuMultiplier * 2 * 1000 * 1000) | 0;
const MaxCyclesPerFrame = clocksPerSecond / 10;

var screen = new jrvideo();

const canvas = $("#bbcCanvas")[0];
video = new Video(model.isMaster, function paint() {
    canvas.width = 480; 
    canvas.height = 500;
    
    var offset = ((processor.video.regs[12] * 256) + processor.video.regs[13]) - 0x2800;
    var cursorPos = ((processor.video.regs[14] * 256) + processor.video.regs[15]) - 0x2800 - offset;

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

        screen.redraw(ctx, pageBuffer, imgData, cursorPos, processor.video.regs[10]);
    }
});

const audioHandler = new AudioHandler($("#audio-warning"), audioFilterFreq, audioFilterQ, noSeek);

let lastShiftLocation = 1;
let lastCtrlLocation = 1;
let lastAltLocation = 1;

dbgr = new Debugger(video);

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
    } else if (code === utils.keyCodes.F12 || code === utils.keyCodes.BREAK) {
        utils.noteEvent("BREAK pressed");
        processor.setReset(true);
        evt.preventDefault();
    } else if (code === utils.keyCodes.K1 && evt.ctrlKey) {
        utils.noteEvent("Drive 0 selected");
        selectedDrive = 0;
    } else if (code === utils.keyCodes.K2 && evt.ctrlKey) {
        utils.noteEvent("Drive 1 selected");
        selectedDrive = 1;
    } else if (code === utils.keyCodes.D && evt.ctrlKey) {
        utils.noteEvent("Downloading contents of drive " + selectedDrive);
        const a = document.createElement("a");
        document.body.appendChild(a);
        a.style = "display: none";
   
        const blob = new Blob([processor.fdc.drives[selectedDrive].data], {
                type: "application/octet-stream",
            }),
            url = window.URL.createObjectURL(blob);
        a.href = url;
        a.download = processor.fdc.drives[selectedDrive].name;
        a.click();
        window.URL.revokeObjectURL(url);
    } else if (code === utils.keyCodes.B && evt.ctrlKey) {
        utils.noteEvent("User initiated CTRL+BREAK");
        processor.setReset(true);
        evt.preventDefault();
    } else if (code === utils.keyCodes.E && evt.ctrlKey) {
        processor.sysvia.keyDown(utils.keyCodes.ESCAPE, evt.shiftKey);
        utils.noteEvent("User initiated ESCAPE");
        evt.preventDefault();
    } else {
        processor.sysvia.keyDown(code, evt.shiftKey);
        evt.preventDefault();
    } 
}

function keyUp(evt) {
    if (document.activeElement.id === "paste-text") return;
    // Always let the key ups come through. That way we don't cause sticky keys in the debugger.
    let code = keyCode(evt);
    if(code === utils.keyCodes.E && evt.ctrlKey)
    {
        code = utils.keyCodes.ESCAPE;
    }
    if (processor && processor.sysvia) processor.sysvia.keyUp(code);
    if (!running) return;
    if (evt.altKey) {
        const handler = emuKeyHandlers[code];
        if (handler) {
            handler(false, code);
            evt.preventDefault();
        }
    } else if (code === utils.keyCodes.F12 || code === utils.keyCodes.BREAK || (code === utils.keyCodes.B && evt.ctrlKey )) {
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

processor = new Cpu6502(
    model,
    dbgr,
    video,
    audioHandler.soundChip,
    model.hasMusic5000 ? audioHandler.music5000 : null,
    emulationConfig
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
    console.log(context);
    console.log(error);
    
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

$("#hard-reset").click(function (event) {
    processor.reset(true);
    event.preventDefault();
});

$("#soft-reset").click(function (event) {
    processor.reset(false);
    event.preventDefault();
});

function guessModelFromUrl() {
    return "BBCDFS";
}

document.addEventListener('dragover', (e) => {
    e.preventDefault()
});
document.addEventListener('drop', (e) => {
    e.preventDefault();    
   
    const reader = new FileReader();
    const fileName = e.dataTransfer.files[0].name;
    reader.readAsArrayBuffer(e.dataTransfer.files[0]);
    reader.onload = function (event) {
        // Either load a .t42 teletext stream, or a disk image into drive 1
        if(fileName.toUpperCase().endsWith(".T42"))
        {
            processor.teletextAdaptor.loadUserChannelStream(fileName, event.target.result);
        }
        else
        {
            processor.fdc.loadDisc(selectedDrive, disc.discFor(processor.fdc, fileName, new Uint8Array(event.target.result)), fileName);
        }
    };    
})

const startPromise = Promise.all([audioHandler.initialise(), processor.initialise()]).then(function () {
    // Ideally would start the loads first. But their completion needs the FDC from the processor
    const imageLoads = [];
    if (discImage)
        imageLoads.push(
            loadDiscImage(discImage).then(function (disc) {
                processor.fdc.loadDisc(0, disc, discImage);
            })
        );
    if (secondDiscImage)
        imageLoads.push(
            loadDiscImage(secondDiscImage).then(function (disc) {
                processor.fdc.loadDisc(1, disc, secondDiscImage);
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

function draw(now) {
    if (!running) {
        last = 0;
        return;
    }
    // If we got here via setTimeout, we don't get passed the time.
    if (now === undefined) {
        now = window.performance.now();
    }

    const speedy = fastAsPossible;
    
    window.requestAnimationFrame(draw);
   
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
        } catch (e) {
            running = false;
            utils.noteEvent(e.stack);
            dbgr.debug(processor.pc);
            throw e;
        }
        if (stepEmuWhenPaused) {
            stop(false);
            stepEmuWhenPaused = false;
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
        if (running) {
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
