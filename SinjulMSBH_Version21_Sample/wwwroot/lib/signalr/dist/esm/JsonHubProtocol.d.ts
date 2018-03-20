import { IHubProtocol, ProtocolType, HubMessage } from "./IHubProtocol";
export declare const JSON_HUB_PROTOCOL_NAME: string;
export declare class JsonHubProtocol implements IHubProtocol {
    readonly name: string;
    readonly type: ProtocolType;
    parseMessages(input: string): HubMessage[];
    writeMessage(message: HubMessage): string;
}
