import { video } from "./video.js";

(function () {
    // Page initialisation
    $(window).on("load", function () {

    // Canvas
    const canvas = document.querySelector('canvas');
    canvas.width = 256; 
    canvas.height = 192;
    
    var screen = new video();
    const ctx = canvas.getContext('2d');
    var imgData = ctx.createImageData(canvas.width, canvas.height);

    var lastPageRefresh = false;
    
    var snafile = new Uint8Array(49179);
  
    // Functions
    function stringToUint8Array(str) {
        if (str instanceof Uint8Array) return str;
        const len = str.length;
        const array = new Uint8Array(len);
        for (let i = 0; i < len; ++i) array[i] = str.charCodeAt(i) & 0xff;
        return array;
    }
    
    function loadDataHttp(url) {
        return new Promise(function (resolve, reject) {
            const request = new XMLHttpRequest();
            request.open("GET", url, true);
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

    function timerTrigger(time) {
        if(time - lastPageRefresh >= 20) { // Refresh every 50htz
            screen.redraw(ctx, snafile, imgData);
            lastPageRefresh = time;
        }

        requestAnimationFrame(timerTrigger);
    }

    // Start the emulation
    loadDataHttp(".\\sna\\Jetpac.htm").then(function (data) {
        snafile = data;
    });


    requestAnimationFrame(timerTrigger);
  });
  })();