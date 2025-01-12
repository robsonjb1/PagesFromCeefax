import { Music5000 } from "./music5000.js";

export class AudioHandler {
    constructor() {
        // Initialise Music 5000 audio context
        this.audioContextM5000 = new AudioContext({ sampleRate: 46875 });
        if (this.audioContextM5000 && this.audioContextM5000.audioWorklet) {
            this.music5000 = new Music5000((buffer) => this._onBufferMusic5000(buffer));

            this.audioContextM5000.audioWorklet.addModule("./js/music5000-worklet.js").then(() => {
                this._music5000workletNode = new AudioWorkletNode(this.audioContextM5000, "music5000", {
                    outputChannelCount: [2],
                });
                this._music5000workletNode.connect(this.audioContextM5000.destination);
            });
        } 
    }

    // Recent browsers, particularly Safari and Chrome, require a user
    // interaction in order to enable sound playback.
    // Recent browsers, particularly Safari and Chrome, require a user interaction in order to enable sound playback.
    async tryResume() {
        if (this.audioContextM5000) await this.audioContextM5000.resume();
    }

    _onBufferMusic5000(buffer) {
        if (this._music5000workletNode) this._music5000workletNode.port.postMessage(buffer);
    }

}
