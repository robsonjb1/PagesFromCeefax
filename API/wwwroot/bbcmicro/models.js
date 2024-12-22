"use strict";

import { I8271, WD1770 } from "./fdc.js";

class Model {
    constructor(name, synonyms, os, nmos, isMaster, swram, fdc, tube, cmosOverride) {
        this.name = name;
        this.synonyms = synonyms;
        this.os = os;
        this.nmos = nmos;
        this.isMaster = isMaster;
        this.Fdc = fdc;
        this.swram = swram;
        this.isTest = false;
        this.tube = tube;
        this.cmosOverride = cmosOverride;
    }
}

// TODO: semi-bplus-style to get swram for exile hardcoded here
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
const masterSwram = [
    false,
    false,
    false,
    false,
    true,
    true,
    true,
    true,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
];
export const allModels = [
    new Model(
        "101: Model B",
        ["B-DFS1.2"],
        ["os.rom", "BASIC.ROM", "b/DFS-1.2.rom", "b/EDIT.rom"],
        true,
        false,
        beebSwram,
        I8271
    ),
    new Model(
        "102: Model B Tube 6502",
        ["B-DFS1.2Tube"],
        ["os.rom", "BASIC.ROM", "b/DFS-2.26.rom", "b/ANFS-4.18.rom", "b/ADFS-1.30.rom"],
        true,
        false,
        beebSwram,
        WD1770
    ),
    new Model("103: BBC Master 128", ["Master"], ["master/mos3.20"], false, true, masterSwram, WD1770, null),
    new Model("104: BBC Master Turbo", ["MasterTurbo"], ["master/mos3.20"], false, true, masterSwram, WD1770, null),
    new Model("Tube65c02", [], ["tube/6502Tube.rom"], false, false), // Although this can not be explicitly selected as a model, it is required by the configuration builder later
    new Model("Tube65c102", [], ["tube/65c102Tube.rom"], false, false), // Although this can not be explicitly selected as a model, it is required by the configuration builder later
];

export function findModel(name) {
    name = name.toLowerCase();
    for (let i = 0; i < allModels.length; ++i) {
        const model = allModels[i];
        if (model.name.toLowerCase() === name) return model;
        for (let j = 0; j < model.synonyms.length; ++j) {
            if (model.synonyms[j].toLowerCase() === name) return model;
        }
    }
    return null;
}

export const TEST_6502 = new Model("TEST", ["TEST"], [], true, false, beebSwram, I8271);
TEST_6502.isTest = true;
export const TEST_65C12 = new Model("TEST", ["TEST"], [], false, false, masterSwram, I8271);
TEST_65C12.isTest = true;

export const basicOnly = new Model("Basic only", ["Basic only"], ["master/mos3.20"], false, true, masterSwram, WD1770);
