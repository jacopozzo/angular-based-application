"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var signalr_1 = require("@aspnet/signalr");
var AppComponent = (function () {
    function AppComponent() {
        var _this = this;
        this.users = [];
        this.messages = [];
        this.message = "";
        this.nick = "";
        this.sendTo = "";
        this.upMessage = function (nick, message, oneToOne) {
            if (oneToOne === void 0) { oneToOne = false; }
            nick = oneToOne ? nick + " sent to you" : nick;
            var text = nick + ": " + message;
            _this.messages.push(text);
        };
        console.log('AppComponent -> constructor');
    }
    AppComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.nick = window.prompt("What's your nickname?", "J4C0x");
        this.hubConnection = new signalr_1.HubConnection("/chat");
        this.configureHubBehaviour();
        this.hubConnection
            .start()
            .then(function () { console.log('Connection started!'); _this.connect(); alert("Connected!"); })
            .catch(function (err) { console.log('Error while establishing connection ', err); alert("Unable to connect!"); });
    };
    AppComponent.prototype.connect = function () {
        this.hubConnection.invoke("connect", this.nick).catch(function (err) { return console.error(err); });
        ;
    };
    AppComponent.prototype.sendMessage = function () {
        if (this.sendTo === "" || !this.sendTo || this.sendTo === "All")
            this.hubConnection
                .invoke('sendToAll', this.nick, this.message)
                .catch(function (err) { return console.error(err); });
        else
            this.hubConnection.invoke('sendToClient', this.sendTo, this.message)
                .catch(function (err) { return console.error(err); });
    };
    AppComponent.prototype.configureHubBehaviour = function () {
        var _this = this;
        this.hubConnection.on('sendToAll', function (nick, message) {
            _this.upMessage(nick, message);
        });
        this.hubConnection.on('sendToClient', function (nick, message) {
            _this.upMessage(nick, message, true);
        });
        this.hubConnection.on('getConnectedUsers', function (info) {
            _this.users = info;
        });
    };
    AppComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            moduleId: module.id,
            templateUrl: "app.component.html",
            providers: []
        }),
        __metadata("design:paramtypes", [])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
var ClientInfo = (function () {
    function ClientInfo() {
    }
    return ClientInfo;
}());
exports.ClientInfo = ClientInfo;
//# sourceMappingURL=app.component.js.map