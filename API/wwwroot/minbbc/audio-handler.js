import { SoundChip } from "./soundchip.js";
import { Music5000 } from "./music5000.js";

export class AudioHandler {
    constructor(warningNode, audioFilterFreq, audioFilterQ) {
        this.warningNode = warningNode;

         /*global webkitAudioContext*/
         this.audioContext =
         typeof AudioContext !== "undefined"
             ? new AudioContext({ sampleRate: 500000 })
             : typeof webkitAudioContext !== "undefined"
               ? new webkitAudioContext({ sampleRate: 500000 })
               : null;
        this._soundchipWorkletNode = null;
        if (this.audioContext && this.audioContext.audioWorklet) {
            this.soundChip = new SoundChip((buffer, time) => this._onBuffer(buffer, time));
            this._setup(audioFilterFreq, audioFilterQ).then();
        } 
    
        // Initialise Music 5000 audio context
        this.audioContextM5000 =
            typeof AudioContext !== "undefined"
                ? new AudioContext({ sampleRate: 46875 })
                : typeof webkitAudioContext !== "undefined"
                  ? new webkitAudioContext({ sampleRate: 46875 })
                  : null;

        if (this.audioContextM5000 && this.audioContextM5000.audioWorklet) {
            this.music5000 = new Music5000((buffer) => this._onBufferMusic5000(buffer));

            this.audioContextM5000.audioWorklet.addModule("./music5000-worklet.js").then(() => {
                this._music5000workletNode = new AudioWorkletNode(this.audioContextM5000, "music5000", {
                    outputChannelCount: [2],
                });
                this._music5000workletNode.connect(this.audioContextM5000.destination);
            });
        } 
    }

    async _setup(audioFilterFreq, audioFilterQ) {
        await this.audioContext.audioWorklet.addModule("./soundchip-worklet.js");
        if (audioFilterFreq !== 0) {
            const filterNode = this.audioContext.createBiquadFilter();
            filterNode.type = "lowpass";
            filterNode.frequency.value = audioFilterFreq;
            filterNode.Q.value = audioFilterQ;
            this._audioDestination = filterNode;
            filterNode.connect(this.audioContext.destination);
        } else {
            this._audioDestination = this.audioContext.destination;
        }

        this._soundchipWorkletNode = new AudioWorkletNode(this.audioContext, "soundchip", {
            outputChannelCount: [1],
        });
        this._soundchipWorkletNode.connect(this._audioDestination);
    }

    // Recent browsers, particularly Safari and Chrome, require a user
    // interaction in order to enable sound playback.
    // Recent browsers, particularly Safari and Chrome, require a user interaction in order to enable sound playback.
    async tryResume() {
        if (this.audioContext) await this.audioContext.resume();
        if (this.audioContextM5000) await this.audioContextM5000.resume();
    }

    _onBuffer(buffer) {
        if (this._soundchipWorkletNode) this._soundchipWorkletNode.port.postMessage(buffer);
    }

    _onBufferMusic5000(buffer) {
        if (this._music5000workletNode) this._music5000workletNode.port.postMessage(buffer);
    }

    mute() {
        this.soundChip.mute();
    }

    unmute() {
        this.soundChip.unmute();
    }
}
