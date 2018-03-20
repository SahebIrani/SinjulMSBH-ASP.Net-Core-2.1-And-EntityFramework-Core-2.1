"use strict";
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var HttpError = /** @class */ (function (_super) {
    __extends(HttpError, _super);
    function HttpError(errorMessage, statusCode) {
        var _this = _super.call(this, errorMessage) || this;
        _this.statusCode = statusCode;
        return _this;
    }
    return HttpError;
}(Error));
exports.HttpError = HttpError;
var TimeoutError = /** @class */ (function (_super) {
    __extends(TimeoutError, _super);
    function TimeoutError(errorMessage) {
        if (errorMessage === void 0) { errorMessage = "A timeout occurred."; }
        return _super.call(this, errorMessage) || this;
    }
    return TimeoutError;
}(Error));
exports.TimeoutError = TimeoutError;
//# sourceMappingURL=Errors.js.map