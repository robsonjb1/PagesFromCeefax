import * as utils from "./utils.js";
import { Video } from "./video.js";
import { Cpu6502 } from "./6502.js";
import * as disc from "./fdc.js";
import { starCat } from "../discs/cat.js";
import { Config } from "./config.js";
import { AudioHandler } from "./audio-handler.js";
import { jrvideo } from "./jrvideo.js";

let processor;
let video;
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
let parsedQuery = {};
let keyLayout = window.localStorage.keyLayout || "physical";

const BBC = utils.BBC;
const emuKeyHandlers = {};
let cpuMultiplier = 1;
let fastAsPossible = false;
let stepEmuWhenPaused = false;
let selectedDrive = 0;

const emulationConfig = {
    keyLayout: keyLayout,
    coProcessor: parsedQuery.coProcessor,
    cpuMultiplier: cpuMultiplier,
    videoCyclesBatch: parsedQuery.videoCyclesBatch,
    extraRoms: extraRoms,
};

const config = new Config();
config.setModel("BBCDFS");
config.setKeyLayout(keyLayout);
config.set65c02(parsedQuery.coProcessor);
config.setTeletext(true);
config.setMusic5000(true);

model = config.model;
const clocksPerSecond = (cpuMultiplier * 2 * 1000 * 1000) | 0;
const MaxCyclesPerFrame = clocksPerSecond / 10;

var screen = new jrvideo();

const teletextCanvas = $("#teletextCanvas")[0];
const graphicsCanvas = $("#graphicsCanvas")[0];
video = new Video(model.isMaster, function paint() {
    if(processor.video.teletextMode)
    {
        $("#graphicsCanvas").hide();
        $("#teletextCanvas").show();

        teletextCanvas.width = 480; 
        teletextCanvas.height = 500;

        const ctx = teletextCanvas.getContext('2d');
        var imgData = ctx.createImageData(teletextCanvas.width, teletextCanvas.height);
    
        screen.teletextRedraw(ctx, imgData, processor);
    }
    else
    {
        $("#teletextCanvas").hide();
        $("#graphicsCanvas").show();
       
        graphicsCanvas.width = 640; 
        graphicsCanvas.height = 256;

        const ctx = graphicsCanvas.getContext('2d');
        var imgData = ctx.createImageData(graphicsCanvas.width, graphicsCanvas.height);

        screen.graphicsModeRedraw(ctx, imgData, processor);
    }
});

const audioHandler = new AudioHandler();

let lastShiftLocation = 1;
let lastCtrlLocation = 1;
let lastAltLocation = 1;

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


function keyDown(evt) {
    audioHandler.tryResume();
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

$(window).blur(function () {
    if (processor.sysvia) processor.sysvia.clearKeys();
});

document.onkeydown = keyDown;
document.onkeyup = keyUp;

processor = new Cpu6502(
    model,
    video,
    audioHandler.music5000,
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

function showError(context, error) {
    console.log(context);
    console.log(error);
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

    return disc.load("discs/" + discImage).then(function (discData) {
        return disc.discFor(processor.fdc, discImage, discData);
    });
}

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

const startPromise = Promise.all([processor.initialise()]).then(function () {
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

function go() {
    running = true;
    run();
}

function stop(debug) {
    running = false;
    processor.stop();
};
