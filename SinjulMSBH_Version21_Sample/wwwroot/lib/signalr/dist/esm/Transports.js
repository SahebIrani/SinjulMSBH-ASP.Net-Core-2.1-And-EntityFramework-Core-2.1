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
import { HttpError, TimeoutError } from "./Errors";
import { LogLevel } from "./ILogger";
import { AbortController } from "./AbortController";
export var TransportType;
(function (TransportType) {
    TransportType[TransportType["WebSockets"] = 0] = "WebSockets";
    TransportType[TransportType["ServerSentEvents"] = 1] = "ServerSentEvents";
    TransportType[TransportType["LongPolling"] = 2] = "LongPolling";
})(TransportType || (TransportType = {}));
export class WebSocketTransport {
    constructor(accessTokenFactory, logger) {
        this.logger = logger;
        this.accessTokenFactory = accessTokenFactory || (() => null);
    }
    connect(url, requestedTransferMode, connection) {
        return new Promise((resolve, reject) => {
            url = url.replace(/^http/, "ws");
            let token = this.accessTokenFactory();
            if (token) {
                url += (url.indexOf("?") < 0 ? "?" : "&") + `access_token=${encodeURIComponent(token)}`;
            }
            let webSocket = new WebSocket(url);
            if (requestedTransferMode == 2 /* Binary */) {
                webSocket.binaryType = "arraybuffer";
            }
            webSocket.onopen = (event) => {
                this.logger.log(LogLevel.Information, `WebSocket connected to ${url}`);
                this.webSocket = webSocket;
                resolve(requestedTransferMode);
            };
            webSocket.onerror = (event) => {
                reject();
            };
            webSocket.onmessage = (message) => {
                this.logger.log(LogLevel.Trace, `(WebSockets transport) data received: ${message.data}`);
                if (this.onreceive) {
                    this.onreceive(message.data);
                }
            };
            webSocket.onclose = (event) => {
                // webSocket will be null if the transport did not start successfully
                if (this.onclose && this.webSocket) {
                    if (event.wasClean === false || event.code !== 1000) {
                        this.onclose(new Error(`Websocket closed with status code: ${event.code} (${event.reason})`));
                    }
                    else {
                        this.onclose();
                    }
                }
            };
        });
    }
    send(data) {
        if (this.webSocket && this.webSocket.readyState === WebSocket.OPEN) {
            this.webSocket.send(data);
            return Promise.resolve();
        }
        return Promise.reject("WebSocket is not in the OPEN state");
    }
    stop() {
        if (this.webSocket) {
            this.webSocket.close();
            this.webSocket = null;
        }
        return Promise.resolve();
    }
}
export class ServerSentEventsTransport {
    constructor(httpClient, accessTokenFactory, logger) {
        this.httpClient = httpClient;
        this.accessTokenFactory = accessTokenFactory || (() => null);
        this.logger = logger;
    }
    connect(url, requestedTransferMode, connection) {
        if (typeof (EventSource) === "undefined") {
            Promise.reject("EventSource not supported by the browser.");
        }
        this.url = url;
        return new Promise((resolve, reject) => {
            let token = this.accessTokenFactory();
            if (token) {
                url += (url.indexOf("?") < 0 ? "?" : "&") + `access_token=${encodeURIComponent(token)}`;
            }
            let eventSource = new EventSource(url);
            try {
                eventSource.onmessage = (e) => {
                    if (this.onreceive) {
                        try {
                            this.logger.log(LogLevel.Trace, `(SSE transport) data received: ${e.data}`);
                            this.onreceive(e.data);
                        }
                        catch (error) {
                            if (this.onclose) {
                                this.onclose(error);
                            }
                            return;
                        }
                    }
                };
                eventSource.onerror = (e) => {
                    reject();
                    // don't report an error if the transport did not start successfully
                    if (this.eventSource && this.onclose) {
                        this.onclose(new Error(e.message || "Error occurred"));
                    }
                };
                eventSource.onopen = () => {
                    this.logger.log(LogLevel.Information, `SSE connected to ${this.url}`);
                    this.eventSource = eventSource;
                    // SSE is a text protocol
                    resolve(1 /* Text */);
                };
            }
            catch (e) {
                return Promise.reject(e);
            }
        });
    }
    send(data) {
        return __awaiter(this, void 0, void 0, function* () {
            return send(this.httpClient, this.url, this.accessTokenFactory, data);
        });
    }
    stop() {
        if (this.eventSource) {
            this.eventSource.close();
            this.eventSource = null;
        }
        return Promise.resolve();
    }
}
export class LongPollingTransport {
    constructor(httpClient, accessTokenFactory, logger) {
        this.httpClient = httpClient;
        this.accessTokenFactory = accessTokenFactory || (() => null);
        this.logger = logger;
        this.pollAbort = new AbortController();
    }
    connect(url, requestedTransferMode, connection) {
        this.url = url;
        // Set a flag indicating we have inherent keep-alive in this transport.
        connection.features.inherentKeepAlive = true;
        if (requestedTransferMode === 2 /* Binary */ && (typeof new XMLHttpRequest().responseType !== "string")) {
            // This will work if we fix: https://github.com/aspnet/SignalR/issues/742
            throw new Error("Binary protocols over XmlHttpRequest not implementing advanced features are not supported.");
        }
        this.poll(this.url, requestedTransferMode);
        return Promise.resolve(requestedTransferMode);
    }
    poll(url, transferMode) {
        return __awaiter(this, void 0, void 0, function* () {
            let pollOptions = {
                timeout: 120000,
                abortSignal: this.pollAbort.signal,
                headers: new Map(),
            };
            if (transferMode === 2 /* Binary */) {
                pollOptions.responseType = "arraybuffer";
            }
            let token = this.accessTokenFactory();
            if (token) {
                pollOptions.headers.set("Authorization", `Bearer ${token}`);
            }
            while (!this.pollAbort.signal.aborted) {
                try {
                    let pollUrl = `${url}&_=${Date.now()}`;
                    this.logger.log(LogLevel.Trace, `(LongPolling transport) polling: ${pollUrl}`);
                    let response = yield this.httpClient.get(pollUrl, pollOptions);
                    if (response.statusCode === 204) {
                        this.logger.log(LogLevel.Information, "(LongPolling transport) Poll terminated by server");
                        // Poll terminated by server
                        if (this.onclose) {
                            this.onclose();
                        }
                        this.pollAbort.abort();
                    }
                    else if (response.statusCode !== 200) {
                        this.logger.log(LogLevel.Error, `(LongPolling transport) Unexpected response code: ${response.statusCode}`);
                        // Unexpected status code
                        if (this.onclose) {
                            this.onclose(new HttpError(response.statusText, response.statusCode));
                        }
                        this.pollAbort.abort();
                    }
                    else {
                        // Process the response
                        if (response.content) {
                            this.logger.log(LogLevel.Trace, `(LongPolling transport) data received: ${response.content}`);
                            if (this.onreceive) {
                                this.onreceive(response.content);
                            }
                        }
                        else {
                            // This is another way timeout manifest.
                            this.logger.log(LogLevel.Trace, "(LongPolling transport) Poll timed out, reissuing.");
                        }
                    }
                }
                catch (e) {
                    if (e instanceof TimeoutError) {
                        // Ignore timeouts and reissue the poll.
                        this.logger.log(LogLevel.Trace, "(LongPolling transport) Poll timed out, reissuing.");
                    }
                    else {
                        // Close the connection with the error as the result.
                        if (this.onclose) {
                            this.onclose(e);
                        }
                        this.pollAbort.abort();
                    }
                }
            }
        });
    }
    send(data) {
        return __awaiter(this, void 0, void 0, function* () {
            return send(this.httpClient, this.url, this.accessTokenFactory, data);
        });
    }
    stop() {
        this.pollAbort.abort();
        return Promise.resolve();
    }
}
function send(httpClient, url, accessTokenFactory, content) {
    return __awaiter(this, void 0, void 0, function* () {
        let headers;
        let token = accessTokenFactory();
        if (token) {
            headers = new Map();
            headers.set("Authorization", `Bearer ${accessTokenFactory()}`);
        }
        yield httpClient.post(url, {
            content,
            headers
        });
    });
}
//# sourceMappingURL=Transports.js.map