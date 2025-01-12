"use strict";
import { findModel } from "./models.js";

export class Config {
    constructor() {
        this.changed = {};
        this.model = null;
        this.coProcessor = null;

        $(".model-menu a").on("click", (e) => {
            this.changed.model = $(e.target).attr("data-target");
            this.setDropdownText($(e.target).text());
        });

        $("#65c02").on("click", () => {
            this.changed.coProcessor = $("#65c02").prop("checked");
        });

        $("#hasTeletextAdaptor").on("click", () => {
            this.changed.hasTeletextAdaptor = $("#hasTeletextAdaptor").prop("checked");
        });

        $("#hasEconet").on("click", () => {
            this.changed.hasEconet = $("#hasEconet").prop("checked");
        });

        $("#hasMusic5000").on("click", () => {
            this.changed.hasMusic5000 = $("#hasMusic5000").prop("checked");
        });

        $(".keyboard-menu a").on("click", (e) => {
            const keyLayout = $(e.target).attr("data-target");
            this.changed.keyLayout = keyLayout;
            this.setKeyLayout(keyLayout);
        });
    }

    setModel(modelName) {
        this.model = findModel(modelName);
        $(".bbc-model").text(this.model.name);
    }

    setKeyLayout(keyLayout) {
        $(".keyboard-layout").text(keyLayout[0].toUpperCase() + keyLayout.substr(1));
    }

    set65c02(enabled) {
        enabled = !!enabled;
        $("#65c02").prop("checked", enabled);
        this.model.tube = enabled ? (this.model.isMaster ? findModel("Tube65c102") : findModel("Tube65c02")) : null;
    }

    setEconet(enabled) {
        enabled = !!enabled;
        $("#hasEconet").prop("checked", enabled);
        this.model.hasEconet = enabled;

        if (enabled && this.model.isMaster) {
            this.addRemoveROM("master/anfs-4.25.rom", true);
        }
    }

    setMusic5000(enabled) {
        enabled = !!enabled;
        $("#hasMusic5000").prop("checked", enabled);
        this.model.hasMusic5000 = enabled;
        this.addRemoveROM("ample.rom", enabled);
    }

    setTeletext(enabled) {
        enabled = !!enabled;
        $("#hasTeletextAdaptor").prop("checked", enabled);
        this.model.hasTeletextAdaptor = enabled;
        this.addRemoveROM("ATS-3.0.rom", enabled);
    }

    setDropdownText(modelName) {
        $("#bbc-model-dropdown .bbc-model").text(modelName);
    }

    addRemoveROM(romName, required) {
        if (required && !this.model.os.includes(romName)) {
            this.model.os.push(romName);
        } else {
            let pos = this.model.os.indexOf(romName);
            if (pos !== -1) {
                this.model.os.splice(pos, 1);
            }
        }
    }
}
