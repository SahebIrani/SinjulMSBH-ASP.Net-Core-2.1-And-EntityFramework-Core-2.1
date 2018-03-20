"use strict";
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = y[op[0] & 2 ? "return" : op[0] ? "throw" : "next"]) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [0, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
var Transports_1 = require("./Transports");
var HttpClient_1 = require("./HttpClient");
var ILogger_1 = require("./ILogger");
var Loggers_1 = require("./Loggers");
var HttpConnection = /** @class */ (function () {
    function HttpConnection(url, options) {
        if (options === void 0) { options = {}; }
        this.features = {};
        this.logger = Loggers_1.LoggerFactory.createLogger(options.logger);
        this.baseUrl = this.resolveUrl(url);
        options = options || {};
        options.accessTokenFactory = options.accessTokenFactory || (function () { return null; });
        this.httpClient = options.httpClient || new HttpClient_1.DefaultHttpClient();
        this.connectionState = 2 /* Disconnected */;
        this.options = options;
    }
    HttpConnection.prototype.start = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                if (this.connectionState !== 2 /* Disconnected */) {
                    return [2 /*return*/, Promise.reject(new Error("Cannot start a connection that is not in the 'Disconnected' state."))];
                }
                this.connectionState = 0 /* Connecting */;
                this.startPromise = this.startInternal();
                return [2 /*return*/, this.startPromise];
            });
        });
    };
    HttpConnection.prototype.startInternal = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _this = this;
            var headers, token, negotiatePayload, negotiateResponse, requestedTransferMode, _a, e_1;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _b.trys.push([0, 5, , 6]);
                        if (!(this.options.transport === Transports_1.TransportType.WebSockets)) return [3 /*break*/, 1];
                        // No need to add a connection ID in this case
                        this.url = this.baseUrl;
                        this.transport = this.createTransport(this.options.transport, [Transports_1.TransportType[Transports_1.TransportType.WebSockets]]);
                        return [3 /*break*/, 3];
                    case 1:
                        headers = void 0;
                        token = this.options.accessTokenFactory();
                        if (token) {
                            headers = new Map();
                            headers.set("Authorization", "Bearer " + token);
                        }
                        return [4 /*yield*/, this.httpClient.post(this.resolveNegotiateUrl(this.baseUrl), {
                                content: "",
                                headers: headers
                            })];
                    case 2:
                        negotiatePayload = _b.sent();
                        negotiateResponse = JSON.parse(negotiatePayload.content);
                        this.connectionId = negotiateResponse.connectionId;
                        // the user tries to stop the the connection when it is being started
                        if (this.connectionState == 2 /* Disconnected */) {
                            return [2 /*return*/];
                        }
                        if (this.connectionId) {
                            this.url = this.baseUrl + (this.baseUrl.indexOf("?") === -1 ? "?" : "&") + ("id=" + this.connectionId);
                            this.transport = this.createTransport(this.options.transport, negotiateResponse.availableTransports);
                        }
                        _b.label = 3;
                    case 3:
                        this.transport.onreceive = this.onreceive;
                        this.transport.onclose = function (e) { return _this.stopConnection(true, e); };
                        requestedTransferMode = this.features.transferMode === 2 /* Binary */
                            ? 2 /* Binary */
                            : 1 /* Text */;
                        _a = this.features;
                        return [4 /*yield*/, this.transport.connect(this.url, requestedTransferMode, this)];
                    case 4:
                        _a.transferMode = _b.sent();
                        // only change the state if we were connecting to not overwrite
                        // the state if the connection is already marked as Disconnected
                        this.changeState(0 /* Connecting */, 1 /* Connected */);
                        return [3 /*break*/, 6];
                    case 5:
                        e_1 = _b.sent();
                        this.logger.log(ILogger_1.LogLevel.Error, "Failed to start the connection. " + e_1);
                        this.connectionState = 2 /* Disconnected */;
                        this.transport = null;
                        throw e_1;
                    case 6:
                        ;
                        return [2 /*return*/];
                }
            });
        });
    };
    HttpConnection.prototype.createTransport = function (transport, availableTransports) {
        if ((transport === null || transport === undefined) && availableTransports.length > 0) {
            transport = Transports_1.TransportType[availableTransports[0]];
        }
        if (transport === Transports_1.TransportType.WebSockets && availableTransports.indexOf(Transports_1.TransportType[transport]) >= 0) {
            return new Transports_1.WebSocketTransport(this.options.accessTokenFactory, this.logger);
        }
        if (transport === Transports_1.TransportType.ServerSentEvents && availableTransports.indexOf(Transports_1.TransportType[transport]) >= 0) {
            return new Transports_1.ServerSentEventsTransport(this.httpClient, this.options.accessTokenFactory, this.logger);
        }
        if (transport === Transports_1.TransportType.LongPolling && availableTransports.indexOf(Transports_1.TransportType[transport]) >= 0) {
            return new Transports_1.LongPollingTransport(this.httpClient, this.options.accessTokenFactory, this.logger);
        }
        if (this.isITransport(transport)) {
            return transport;
        }
        throw new Error("No available transports found.");
    };
    HttpConnection.prototype.isITransport = function (transport) {
        return typeof (transport) === "object" && "connect" in transport;
    };
    HttpConnection.prototype.changeState = function (from, to) {
        if (this.connectionState == from) {
            this.connectionState = to;
            return true;
        }
        return false;
    };
    HttpConnection.prototype.send = function (data) {
        if (this.connectionState != 1 /* Connected */) {
            throw new Error("Cannot send data if the connection is not in the 'Connected' State");
        }
        return this.transport.send(data);
    };
    HttpConnection.prototype.stop = function (error) {
        return __awaiter(this, void 0, void 0, function () {
            var previousState, e_2;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        previousState = this.connectionState;
                        this.connectionState = 2 /* Disconnected */;
                        _a.label = 1;
                    case 1:
                        _a.trys.push([1, 3, , 4]);
                        return [4 /*yield*/, this.startPromise];
                    case 2:
                        _a.sent();
                        return [3 /*break*/, 4];
                    case 3:
                        e_2 = _a.sent();
                        return [3 /*break*/, 4];
                    case 4:
                        this.stopConnection(/*raiseClosed*/ previousState == 1 /* Connected */, error);
                        return [2 /*return*/];
                }
            });
        });
    };
    HttpConnection.prototype.stopConnection = function (raiseClosed, error) {
        if (this.transport) {
            this.transport.stop();
            this.transport = null;
        }
        if (error) {
            this.logger.log(ILogger_1.LogLevel.Error, "Connection disconnected with error '" + error + "'.");
        }
        else {
            this.logger.log(ILogger_1.LogLevel.Information, "Connection disconnected.");
        }
        this.connectionState = 2 /* Disconnected */;
        if (raiseClosed && this.onclose) {
            this.onclose(error);
        }
    };
    HttpConnection.prototype.resolveUrl = function (url) {
        // startsWith is not supported in IE
        if (url.lastIndexOf("https://", 0) === 0 || url.lastIndexOf("http://", 0) === 0) {
            return url;
        }
        if (typeof window === 'undefined' || !window || !window.document) {
            throw new Error("Cannot resolve '" + url + "'.");
        }
        var parser = window.document.createElement("a");
        parser.href = url;
        var baseUrl = (!parser.protocol || parser.protocol === ":")
            ? window.document.location.protocol + "//" + (parser.host || window.document.location.host)
            : parser.protocol + "//" + parser.host;
        if (!url || url[0] != '/') {
            url = '/' + url;
        }
        var normalizedUrl = baseUrl + url;
        this.logger.log(ILogger_1.LogLevel.Information, "Normalizing '" + url + "' to '" + normalizedUrl + "'");
        return normalizedUrl;
    };
    HttpConnection.prototype.resolveNegotiateUrl = function (url) {
        var index = url.indexOf("?");
        var negotiateUrl = url.substring(0, index === -1 ? url.length : index);
        if (negotiateUrl[negotiateUrl.length - 1] !== "/") {
            negotiateUrl += "/";
        }
        negotiateUrl += "negotiate";
        negotiateUrl += index === -1 ? "" : url.substring(index);
        return negotiateUrl;
    };
    return HttpConnection;
}());
exports.HttpConnection = HttpConnection;
//# sourceMappingURL=HttpConnection.js.map