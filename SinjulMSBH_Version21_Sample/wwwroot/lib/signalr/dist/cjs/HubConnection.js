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
var HttpConnection_1 = require("./HttpConnection");
var Observable_1 = require("./Observable");
var JsonHubProtocol_1 = require("./JsonHubProtocol");
exports.JsonHubProtocol = JsonHubProtocol_1.JsonHubProtocol;
var TextMessageFormat_1 = require("./TextMessageFormat");
var Base64EncodedHubProtocol_1 = require("./Base64EncodedHubProtocol");
var ILogger_1 = require("./ILogger");
var Loggers_1 = require("./Loggers");
var DEFAULT_TIMEOUT_IN_MS = 30 * 1000;
var HubConnection = /** @class */ (function () {
    function HubConnection(urlOrConnection, options) {
        if (options === void 0) { options = {}; }
        var _this = this;
        options = options || {};
        this.timeoutInMilliseconds = options.timeoutInMilliseconds || DEFAULT_TIMEOUT_IN_MS;
        if (typeof urlOrConnection === "string") {
            this.connection = new HttpConnection_1.HttpConnection(urlOrConnection, options);
        }
        else {
            this.connection = urlOrConnection;
        }
        this.logger = Loggers_1.LoggerFactory.createLogger(options.logger);
        this.protocol = options.protocol || new JsonHubProtocol_1.JsonHubProtocol();
        this.connection.onreceive = function (data) { return _this.processIncomingData(data); };
        this.connection.onclose = function (error) { return _this.connectionClosed(error); };
        this.callbacks = new Map();
        this.methods = new Map();
        this.closedCallbacks = [];
        this.id = 0;
    }
    HubConnection.prototype.processIncomingData = function (data) {
        if (this.timeoutHandle !== undefined) {
            clearTimeout(this.timeoutHandle);
        }
        // Parse the messages
        var messages = this.protocol.parseMessages(data);
        for (var i = 0; i < messages.length; ++i) {
            var message = messages[i];
            switch (message.type) {
                case 1 /* Invocation */:
                    this.invokeClientMethod(message);
                    break;
                case 2 /* StreamItem */:
                case 3 /* Completion */:
                    var callback = this.callbacks.get(message.invocationId);
                    if (callback != null) {
                        if (message.type === 3 /* Completion */) {
                            this.callbacks.delete(message.invocationId);
                        }
                        callback(message);
                    }
                    break;
                case 6 /* Ping */:
                    // Don't care about pings
                    break;
                default:
                    this.logger.log(ILogger_1.LogLevel.Warning, "Invalid message type: " + data);
                    break;
            }
        }
        this.configureTimeout();
    };
    HubConnection.prototype.configureTimeout = function () {
        var _this = this;
        if (!this.connection.features || !this.connection.features.inherentKeepAlive) {
            // Set the timeout timer
            this.timeoutHandle = setTimeout(function () { return _this.serverTimeout(); }, this.timeoutInMilliseconds);
        }
    };
    HubConnection.prototype.serverTimeout = function () {
        // The server hasn't talked to us in a while. It doesn't like us anymore ... :(
        // Terminate the connection
        this.connection.stop(new Error("Server timeout elapsed without receiving a message from the server."));
    };
    HubConnection.prototype.invokeClientMethod = function (invocationMessage) {
        var _this = this;
        var methods = this.methods.get(invocationMessage.target.toLowerCase());
        if (methods) {
            methods.forEach(function (m) { return m.apply(_this, invocationMessage.arguments); });
            if (invocationMessage.invocationId) {
                // This is not supported in v1. So we return an error to avoid blocking the server waiting for the response.
                var message = "Server requested a response, which is not supported in this version of the client.";
                this.logger.log(ILogger_1.LogLevel.Error, message);
                this.connection.stop(new Error(message));
            }
        }
        else {
            this.logger.log(ILogger_1.LogLevel.Warning, "No client method with the name '" + invocationMessage.target + "' found.");
        }
    };
    HubConnection.prototype.connectionClosed = function (error) {
        var _this = this;
        this.callbacks.forEach(function (callback) {
            callback(undefined, error ? error : new Error("Invocation canceled due to connection being closed."));
        });
        this.callbacks.clear();
        this.closedCallbacks.forEach(function (c) { return c.apply(_this, [error]); });
        this.cleanupTimeout();
    };
    HubConnection.prototype.start = function () {
        return __awaiter(this, void 0, void 0, function () {
            var requestedTransferMode, actualTransferMode;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        requestedTransferMode = (this.protocol.type === 2 /* Binary */)
                            ? 2 /* Binary */
                            : 1 /* Text */;
                        this.connection.features.transferMode = requestedTransferMode;
                        return [4 /*yield*/, this.connection.start()];
                    case 1:
                        _a.sent();
                        actualTransferMode = this.connection.features.transferMode;
                        return [4 /*yield*/, this.connection.send(TextMessageFormat_1.TextMessageFormat.write(JSON.stringify({ protocol: this.protocol.name })))];
                    case 2:
                        _a.sent();
                        this.logger.log(ILogger_1.LogLevel.Information, "Using HubProtocol '" + this.protocol.name + "'.");
                        if (requestedTransferMode === 2 /* Binary */ && actualTransferMode === 1 /* Text */) {
                            this.protocol = new Base64EncodedHubProtocol_1.Base64EncodedHubProtocol(this.protocol);
                        }
                        this.configureTimeout();
                        return [2 /*return*/];
                }
            });
        });
    };
    HubConnection.prototype.stop = function () {
        this.cleanupTimeout();
        return this.connection.stop();
    };
    HubConnection.prototype.stream = function (methodName) {
        var _this = this;
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        var invocationDescriptor = this.createStreamInvocation(methodName, args);
        var subject = new Observable_1.Subject(function () {
            var cancelInvocation = _this.createCancelInvocation(invocationDescriptor.invocationId);
            var message = _this.protocol.writeMessage(cancelInvocation);
            _this.callbacks.delete(invocationDescriptor.invocationId);
            return _this.connection.send(message);
        });
        this.callbacks.set(invocationDescriptor.invocationId, function (invocationEvent, error) {
            if (error) {
                subject.error(error);
                return;
            }
            if (invocationEvent.type === 3 /* Completion */) {
                var completionMessage = invocationEvent;
                if (completionMessage.error) {
                    subject.error(new Error(completionMessage.error));
                }
                else {
                    subject.complete();
                }
            }
            else {
                subject.next(invocationEvent.item);
            }
        });
        var message = this.protocol.writeMessage(invocationDescriptor);
        this.connection.send(message)
            .catch(function (e) {
            subject.error(e);
            _this.callbacks.delete(invocationDescriptor.invocationId);
        });
        return subject;
    };
    HubConnection.prototype.send = function (methodName) {
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        var invocationDescriptor = this.createInvocation(methodName, args, true);
        var message = this.protocol.writeMessage(invocationDescriptor);
        return this.connection.send(message);
    };
    HubConnection.prototype.invoke = function (methodName) {
        var _this = this;
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        var invocationDescriptor = this.createInvocation(methodName, args, false);
        var p = new Promise(function (resolve, reject) {
            _this.callbacks.set(invocationDescriptor.invocationId, function (invocationEvent, error) {
                if (error) {
                    reject(error);
                    return;
                }
                if (invocationEvent.type === 3 /* Completion */) {
                    var completionMessage = invocationEvent;
                    if (completionMessage.error) {
                        reject(new Error(completionMessage.error));
                    }
                    else {
                        resolve(completionMessage.result);
                    }
                }
                else {
                    reject(new Error("Unexpected message type: " + invocationEvent.type));
                }
            });
            var message = _this.protocol.writeMessage(invocationDescriptor);
            _this.connection.send(message)
                .catch(function (e) {
                reject(e);
                _this.callbacks.delete(invocationDescriptor.invocationId);
            });
        });
        return p;
    };
    HubConnection.prototype.on = function (methodName, method) {
        if (!methodName || !method) {
            return;
        }
        methodName = methodName.toLowerCase();
        if (!this.methods.has(methodName)) {
            this.methods.set(methodName, []);
        }
        this.methods.get(methodName).push(method);
    };
    HubConnection.prototype.off = function (methodName, method) {
        if (!methodName || !method) {
            return;
        }
        methodName = methodName.toLowerCase();
        var handlers = this.methods.get(methodName);
        if (!handlers) {
            return;
        }
        var removeIdx = handlers.indexOf(method);
        if (removeIdx != -1) {
            handlers.splice(removeIdx, 1);
        }
    };
    HubConnection.prototype.onclose = function (callback) {
        if (callback) {
            this.closedCallbacks.push(callback);
        }
    };
    HubConnection.prototype.cleanupTimeout = function () {
        if (this.timeoutHandle) {
            clearTimeout(this.timeoutHandle);
        }
    };
    HubConnection.prototype.createInvocation = function (methodName, args, nonblocking) {
        if (nonblocking) {
            return {
                type: 1 /* Invocation */,
                target: methodName,
                arguments: args,
            };
        }
        else {
            var id = this.id;
            this.id++;
            return {
                type: 1 /* Invocation */,
                invocationId: id.toString(),
                target: methodName,
                arguments: args,
            };
        }
    };
    HubConnection.prototype.createStreamInvocation = function (methodName, args) {
        var id = this.id;
        this.id++;
        return {
            type: 4 /* StreamInvocation */,
            invocationId: id.toString(),
            target: methodName,
            arguments: args,
        };
    };
    HubConnection.prototype.createCancelInvocation = function (id) {
        return {
            type: 5 /* CancelInvocation */,
            invocationId: id,
        };
    };
    return HubConnection;
}());
exports.HubConnection = HubConnection;
//# sourceMappingURL=HubConnection.js.map