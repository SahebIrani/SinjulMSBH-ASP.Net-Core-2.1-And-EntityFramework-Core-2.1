import { DataReceived, ConnectionClosed } from "./Common";
import { IConnection } from "./IConnection";
import { ITransport, TransportType } from "./Transports";
import { HttpClient } from "./HttpClient";
import { ILogger, LogLevel } from "./ILogger";
export interface IHttpConnectionOptions {
    httpClient?: HttpClient;
    transport?: TransportType | ITransport;
    logger?: ILogger | LogLevel;
    accessTokenFactory?: () => string;
}
export declare class HttpConnection implements IConnection {
    private connectionState;
    private baseUrl;
    private url;
    private readonly httpClient;
    private readonly logger;
    private readonly options;
    private transport;
    private connectionId;
    private startPromise;
    readonly features: any;
    constructor(url: string, options?: IHttpConnectionOptions);
    start(): Promise<void>;
    private startInternal();
    private createTransport(transport, availableTransports);
    private isITransport(transport);
    private changeState(from, to);
    send(data: any): Promise<void>;
    stop(error?: Error): Promise<void>;
    private stopConnection(raiseClosed, error?);
    private resolveUrl(url);
    private resolveNegotiateUrl(url);
    onreceive: DataReceived;
    onclose: ConnectionClosed;
}
