"use strict";
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
Object.defineProperty(exports, "__esModule", { value: true });
var Base64EncodedHubProtocol = /** @class */ (function () {
    function Base64EncodedHubProtocol(protocol) {
        this.wrappedProtocol = protocol;
        this.name = this.wrappedProtocol.name;
        this.type = 1 /* Text */;
    }
    Base64EncodedHubProtocol.prototype.parseMessages = function (input) {
        // The format of the message is `size:message;`
        var pos = input.indexOf(":");
        if (pos == -1 || input[input.length - 1] != ';') {
            throw new Error("Invalid payload.");
        }
        var lenStr = input.substring(0, pos);
        if (!/^[0-9]+$/.test(lenStr)) {
            throw new Error("Invalid length: '" + lenStr + "'");
        }
        var messageSize = parseInt(lenStr, 10);
        // 2 accounts for ':' after message size and trailing ';'
        if (messageSize != input.length - pos - 2) {
            throw new Error("Invalid message size.");
        }
        var encodedMessage = input.substring(pos + 1, input.length - 1);
        // atob/btoa are browsers APIs but they can be polyfilled. If this becomes problematic we can use
        // base64-js module
        var s = atob(encodedMessage);
        var payload = new Uint8Array(s.length);
        for (var i = 0; i < payload.length; i++) {
            payload[i] = s.charCodeAt(i);
        }
        return this.wrappedProtocol.parseMessages(payload.buffer);
    };
    Base64EncodedHubProtocol.prototype.writeMessage = function (message) {
        var payload = new Uint8Array(this.wrappedProtocol.writeMessage(message));
        var s = "";
        for (var i = 0; i < payload.byteLength; i++) {
            s += String.fromCharCode(payload[i]);
        }
        // atob/btoa are browsers APIs but they can be polyfilled. If this becomes problematic we can use
        // base64-js module
        var encodedMessage = btoa(s);
        return encodedMessage.length.toString() + ":" + encodedMessage + ";";
    };
    return Base64EncodedHubProtocol;
}());
exports.Base64EncodedHubProtocol = Base64EncodedHubProtocol;
//# sourceMappingURL=Base64EncodedHubProtocol.js.map