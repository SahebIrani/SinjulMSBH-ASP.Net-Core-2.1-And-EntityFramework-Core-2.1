"use strict";
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
Object.defineProperty(exports, "__esModule", { value: true });
var TextMessageFormat_1 = require("./TextMessageFormat");
exports.JSON_HUB_PROTOCOL_NAME = "json";
var JsonHubProtocol = /** @class */ (function () {
    function JsonHubProtocol() {
        this.name = exports.JSON_HUB_PROTOCOL_NAME;
        this.type = 1 /* Text */;
    }
    JsonHubProtocol.prototype.parseMessages = function (input) {
        if (!input) {
            return [];
        }
        // Parse the messages
        var messages = TextMessageFormat_1.TextMessageFormat.parse(input);
        var hubMessages = [];
        for (var i = 0; i < messages.length; ++i) {
            hubMessages.push(JSON.parse(messages[i]));
        }
        return hubMessages;
    };
    JsonHubProtocol.prototype.writeMessage = function (message) {
        return TextMessageFormat_1.TextMessageFormat.write(JSON.stringify(message));
    };
    return JsonHubProtocol;
}());
exports.JsonHubProtocol = JsonHubProtocol;
//# sourceMappingURL=JsonHubProtocol.js.map