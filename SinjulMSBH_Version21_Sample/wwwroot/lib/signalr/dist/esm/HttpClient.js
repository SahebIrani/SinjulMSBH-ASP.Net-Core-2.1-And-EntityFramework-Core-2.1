// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
import { TimeoutError, HttpError } from "./Errors";
export class HttpResponse {
    constructor(statusCode, statusText, content) {
        this.statusCode = statusCode;
        this.statusText = statusText;
        this.content = content;
    }
}
export class HttpClient {
    get(url, options) {
        return this.send(Object.assign({}, options, { method: "GET", url: url }));
    }
    post(url, options) {
        return this.send(Object.assign({}, options, { method: "POST", url: url }));
    }
}
export class DefaultHttpClient extends HttpClient {
    send(request) {
        return new Promise((resolve, reject) => {
            let xhr = new XMLHttpRequest();
            xhr.open(request.method, request.url, true);
            xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            if (request.headers) {
                request.headers.forEach((value, header) => xhr.setRequestHeader(header, value));
            }
            if (request.responseType) {
                xhr.responseType = request.responseType;
            }
            if (request.abortSignal) {
                request.abortSignal.onabort = () => {
                    xhr.abort();
                };
            }
            if (request.timeout) {
                xhr.timeout = request.timeout;
            }
            xhr.onload = () => {
                if (request.abortSignal) {
                    request.abortSignal.onabort = null;
                }
                if (xhr.status >= 200 && xhr.status < 300) {
                    resolve(new HttpResponse(xhr.status, xhr.statusText, xhr.response || xhr.responseText));
                }
                else {
                    reject(new HttpError(xhr.statusText, xhr.status));
                }
            };
            xhr.onerror = () => {
                reject(new HttpError(xhr.statusText, xhr.status));
            };
            xhr.ontimeout = () => {
                reject(new TimeoutError());
            };
            xhr.send(request.content || "");
        });
    }
}
//# sourceMappingURL=HttpClient.js.map