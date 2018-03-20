// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
import { TextMessageFormat } from "./TextMessageFormat";
export const JSON_HUB_PROTOCOL_NAME = "json";
export class JsonHubProtocol {
    constructor() {
        this.name = JSON_HUB_PROTOCOL_NAME;
        this.type = 1 /* Text */;
    }
    parseMessages(input) {
        if (!input) {
            return [];
        }
        // Parse the messages
        let messages = TextMessageFormat.parse(input);
        let hubMessages = [];
        for (var i = 0; i < messages.length; ++i) {
            hubMessages.push(JSON.parse(messages[i]));
        }
        return hubMessages;
    }
    writeMessage(message) {
        return TextMessageFormat.write(JSON.stringify(message));
    }
}
//# sourceMappingURL=JsonHubProtocol.js.map