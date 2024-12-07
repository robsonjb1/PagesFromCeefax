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

function ASCIItoCharIndex(char)
{
    if(char >= "A" && char <= "Z")
    {
        return 33 + char.charCodeAt() - 65; // 'A' starts at 33
    }

    if(char >= "a" && char <= "z")
    {
        return 65 + char.charCodeAt() - 97; // 'a' starts at 65
    }

    if(char >= "0" && char <= "9")
    {
        return 16 + char.charCodeAt() - 48; // '0' starts at 16
    }

    switch(char)
    {
        case " ":
            return 0;
            break;
        case ":":
            return 26;
            break;
        case "/":
            return 15;
            break;
        default:
            return 0;
    }
}
