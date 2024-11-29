///////////////////////////////////////////////////////////////////////////////
/// @file utils.js
///
/// @brief Utility functions for the MinZX 48K Spectrum emulator
///
/// @author David Crespo Tascon
///
/// @copyright (c) David Crespo Tascon
///  This code is released under the MIT license,
///  a copy of which is available in the associated LICENSE file,
///  or at http://opensource.org/licenses/MIT
///////////////////////////////////////////////////////////////////////////////

"use strict";

// load from remote URL as Uint8Array passed to callback
function loadRemoteBinaryFile(url, callback)
{
    let req = new XMLHttpRequest();
    req.responseType = 'arraybuffer';
    req.addEventListener('load', function() {
        if (this.status != 200) {
            console.error('loadRemoteBinaryFile: error ' + this.status + ' while trying to read url ' + url);
            callback(null);
            return;
        }
        let arrayBuffer = this.response;
        let byteArray = new Uint8Array(arrayBuffer);
        callback(byteArray);
    });
    req.open('get', url);
    req.send();
}
