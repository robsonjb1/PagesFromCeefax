import * as utils from "./utils.js";
import { Video } from "./video.js";
import { Cpu6502 } from "./6502.js";
import * as disc from "./fdc.js";
import { Display } from "./display.js";
import { Music5000 } from "./music5000.js";

// Initialise screen
let screen = new Display();
const teletextCanvas = $("#teletextCanvas")[0];
const graphicsCanvas = $("#graphicsCanvas")[0];
let video = new Video(teletextCanvas, graphicsCanvas, screen);

// Initialise Music 5000
let audioContextM5000 = new AudioContext({ sampleRate: 46875 });
let music5000 = new Music5000((buffer) => onBufferMusic5000(buffer));
let music5000workletNode = null;

audioContextM5000.audioWorklet.addModule("./js/music5000-worklet.js").then(() => {
    music5000workletNode = new AudioWorkletNode(audioContextM5000, "music5000", {
        outputChannelCount: [2],
    });
    music5000workletNode.connect(audioContextM5000.destination);
});

function onBufferMusic5000(buffer) {
    if (music5000workletNode) music5000workletNode.port.postMessage(buffer);
}

let processor = new Cpu6502(
    video,
    music5000
);

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
    if (audioContextM5000) {
        audioContextM5000.resume();
    }

    const code = keyCode(evt);
    if (code === utils.keyCodes.F12 || code === utils.keyCodes.BREAK) {
        utils.noteEvent("BREAK pressed");
        processor.setReset(true);
        evt.preventDefault();
    } else if (code === utils.keyCodes.K1 && evt.ctrlKey) {
        utils.noteEvent("Drive 0 selected");
        selectedDrive = 0;
    } else if (code === utils.keyCodes.K2 && evt.ctrlKey) {
        utils.noteEvent("Drive 1 selected");
        selectedDrive = 1;
    } else if (code === utils.keyCodes.S && evt.ctrlKey) {
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
    if (code === utils.keyCodes.F12 || code === utils.keyCodes.BREAK || (code === utils.keyCodes.B && evt.ctrlKey )) {
        processor.setReset(false);
    }
    evt.preventDefault();
}

$(window).blur(function () {
    if (processor.sysvia) processor.sysvia.clearKeys();
});

document.onkeydown = keyDown;
document.onkeyup = keyUp;

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
   
    return disc.load("discs/" + discImage).then(function (discData) {
        return disc.discFor(processor.fdc, discImage, discData);
    });
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
        // Either load a .t42 teletext stream or a disk image
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

// Initialise drives 0 and 1
let firstDiscImage = "Music5000.dsd";
let secondDiscImage = "Data.dsd";
let selectedDrive = 0;

const startPromise = Promise.all([processor.initialise()]).then(function () {
    // Ideally would start the loads first. But their completion needs the FDC from the processor
    const imageLoads = [];
    imageLoads.push(
        loadDiscImage(firstDiscImage).then(function (disc) {
            processor.fdc.loadDisc(0, disc, firstDiscImage);
        })
    );
    imageLoads.push(
        loadDiscImage(secondDiscImage).then(function (disc) {
            processor.fdc.loadDisc(1, disc, secondDiscImage);
        })
    );

    return Promise.all(imageLoads);
});

// Start the emulation
startPromise.then(
    function () {
        go();
    },
    function (error) {
        console.log(error);
    }
);

const clocksPerSecond = (2 * 1000 * 1000) | 0;
const MaxCyclesPerFrame = clocksPerSecond / 10;
let last = 0;

function draw(now) {
    now = window.performance.now();
    
    window.requestAnimationFrame(draw);
    if (last !== 0) {
        let cycles;
        const sinceLast = now - last;
        cycles = (sinceLast * clocksPerSecond) / 1000;
        cycles = Math.min(cycles, MaxCyclesPerFrame);
        cycles |= 0;
        try {
            if (!processor.execute(cycles)) {
                stop();
            }
        } catch (e) {
            utils.noteEvent(e.stack);
            throw e;
        }
    }
    last = now;
}

function run() {
    window.requestAnimationFrame(draw);
}

function go() {
    run();
}

function stop() {
    processor.stop();
};
