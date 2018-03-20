// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { TransportType, WebSocketTransport, ServerSentEventsTransport, LongPollingTransport } from "./Transports";
import { DefaultHttpClient } from "./HttpClient";
import { LogLevel } from "./ILogger";
import { LoggerFactory } from "./Loggers";
export class HttpConnection {
    constructor(url, options = {}) {
        this.features = {};
        this.logger = LoggerFactory.createLogger(options.logger);
        this.baseUrl = this.resolveUrl(url);
        options = options || {};
        options.accessTokenFactory = options.accessTokenFactory || (() => null);
        this.httpClient = options.httpClient || new DefaultHttpClient();
        this.connectionState = 2 /* Disconnected */;
        this.options = options;
    }
    start() {
        return __awaiter(this, void 0, void 0, function* () {
            if (this.connectionState !== 2 /* Disconnected */) {
                return Promise.reject(new Error("Cannot start a connection that is not in the 'Disconnected' state."));
            }
            this.connectionState = 0 /* Connecting */;
            this.startPromise = this.startInternal();
            return this.startPromise;
        });
    }
    startInternal() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                if (this.options.transport === TransportType.WebSockets) {
                    // No need to add a connection ID in this case
                    this.url = this.baseUrl;
                    this.transport = this.createTransport(this.options.transport, [TransportType[TransportType.WebSockets]]);
                }
                else {
                    let headers;
                    let token = this.options.accessTokenFactory();
                    if (token) {
                        headers = new Map();
                        headers.set("Authorization", `Bearer ${token}`);
                    }
                    let negotiatePayload = yield this.httpClient.post(this.resolveNegotiateUrl(this.baseUrl), {
                        content: "",
                        headers
                    });
                    let negotiateResponse = JSON.parse(negotiatePayload.content);
                    this.connectionId = negotiateResponse.connectionId;
                    // the user tries to stop the the connection when it is being started
                    if (this.connectionState == 2 /* Disconnected */) {
                        return;
                    }
                    if (this.connectionId) {
                        this.url = this.baseUrl + (this.baseUrl.indexOf("?") === -1 ? "?" : "&") + `id=${this.connectionId}`;
                        this.transport = this.createTransport(this.options.transport, negotiateResponse.availableTransports);
                    }
                }
                this.transport.onreceive = this.onreceive;
                this.transport.onclose = e => this.stopConnection(true, e);
                let requestedTransferMode = this.features.transferMode === 2 /* Binary */
                    ? 2 /* Binary */
                    : 1 /* Text */;
                this.features.transferMode = yield this.transport.connect(this.url, requestedTransferMode, this);
                // only change the state if we were connecting to not overwrite
                // the state if the connection is already marked as Disconnected
                this.changeState(0 /* Connecting */, 1 /* Connected */);
            }
            catch (e) {
                this.logger.log(LogLevel.Error, "Failed to start the connection. " + e);
                this.connectionState = 2 /* Disconnected */;
                this.transport = null;
                throw e;
            }
            ;
        });
    }
    createTransport(transport, availableTransports) {
        if ((transport === null || transport === undefined) && availableTransports.length > 0) {
            transport = TransportType[availableTransports[0]];
        }
        if (transport === TransportType.WebSockets && availableTransports.indexOf(TransportType[transport]) >= 0) {
            return new WebSocketTransport(this.options.accessTokenFactory, this.logger);
        }
        if (transport === TransportType.ServerSentEvents && availableTransports.indexOf(TransportType[transport]) >= 0) {
            return new ServerSentEventsTransport(this.httpClient, this.options.accessTokenFactory, this.logger);
        }
        if (transport === TransportType.LongPolling && availableTransports.indexOf(TransportType[transport]) >= 0) {
            return new LongPollingTransport(this.httpClient, this.options.accessTokenFactory, this.logger);
        }
        if (this.isITransport(transport)) {
            return transport;
        }
        throw new Error("No available transports found.");
    }
    isITransport(transport) {
        return typeof (transport) === "object" && "connect" in transport;
    }
    changeState(from, to) {
        if (this.connectionState == from) {
            this.connectionState = to;
            return true;
        }
        return false;
    }
    send(data) {
        if (this.connectionState != 1 /* Connected */) {
            throw new Error("Cannot send data if the connection is not in the 'Connected' State");
        }
        return this.transport.send(data);
    }
    stop(error) {
        return __awaiter(this, void 0, void 0, function* () {
            let previousState = this.connectionState;
            this.connectionState = 2 /* Disconnected */;
            try {
                yield this.startPromise;
            }
            catch (e) {
                // this exception is returned to the user as a rejected Promise from the start method
            }
            this.stopConnection(/*raiseClosed*/ previousState == 1 /* Connected */, error);
        });
    }
    stopConnection(raiseClosed, error) {
        if (this.transport) {
            this.transport.stop();
            this.transport = null;
        }
        if (error) {
            this.logger.log(LogLevel.Error, `Connection disconnected with error '${error}'.`);
        }
        else {
            this.logger.log(LogLevel.Information, "Connection disconnected.");
        }
        this.connectionState = 2 /* Disconnected */;
        if (raiseClosed && this.onclose) {
            this.onclose(error);
        }
    }
    resolveUrl(url) {
        // startsWith is not supported in IE
        if (url.lastIndexOf("https://", 0) === 0 || url.lastIndexOf("http://", 0) === 0) {
            return url;
        }
        if (typeof window === 'undefined' || !window || !window.document) {
            throw new Error(`Cannot resolve '${url}'.`);
        }
        let parser = window.document.createElement("a");
        parser.href = url;
        let baseUrl = (!parser.protocol || parser.protocol === ":")
            ? `${window.document.location.protocol}//${(parser.host || window.document.location.host)}`
            : `${parser.protocol}//${parser.host}`;
        if (!url || url[0] != '/') {
            url = '/' + url;
        }
        let normalizedUrl = baseUrl + url;
        this.logger.log(LogLevel.Information, `Normalizing '${url}' to '${normalizedUrl}'`);
        return normalizedUrl;
    }
    resolveNegotiateUrl(url) {
        let index = url.indexOf("?");
        let negotiateUrl = url.substring(0, index === -1 ? url.length : index);
        if (negotiateUrl[negotiateUrl.length - 1] !== "/") {
            negotiateUrl += "/";
        }
        negotiateUrl += "negotiate";
        negotiateUrl += index === -1 ? "" : url.substring(index);
        return negotiateUrl;
    }
}
//# sourceMappingURL=HttpConnection.js.map