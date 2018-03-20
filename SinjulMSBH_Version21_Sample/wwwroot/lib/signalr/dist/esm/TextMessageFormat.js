// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
export class TextMessageFormat {
    static write(output) {
        return `${output}${TextMessageFormat.RecordSeparator}`;
    }
    static parse(input) {
        if (input[input.length - 1] != TextMessageFormat.RecordSeparator) {
            throw new Error("Message is incomplete.");
        }
        let messages = input.split(TextMessageFormat.RecordSeparator);
        messages.pop();
        return messages;
    }
}
TextMessageFormat.RecordSeparator = String.fromCharCode(0x1e);
//# sourceMappingURL=TextMessageFormat.js.map