import { Component, OnInit } from '@angular/core';
import { HubConnection } from "@aspnet/signalr";
@Component({
    selector: 'my-app',
    moduleId: module.id,
    templateUrl: "app.component.html",
    providers: []
})
export class AppComponent implements OnInit {
    private hubConnection: HubConnection;
    users: ClientInfo[] = [];
    messages: string[] = [];
    message: string = "";
    nick: string = "";
    sendTo: string = "";
    constructor() {
        console.log('AppComponent -> constructor');
    }

    ngOnInit() {

        this.nick = window.prompt("What's your nickname?", "J4C0x");

        this.hubConnection = new HubConnection("/chat");

        this.configureHubBehaviour();

        this.hubConnection
            .start()
            .then(() => { console.log('Connection started!'); this.connect(); alert("Connected!"); })
            .catch(err => { console.log('Error while establishing connection ', err); alert("Unable to connect!"); });
    }

    connect() {
        this.hubConnection.invoke("connect", this.nick).catch(err => console.error(err));;
    }

    sendMessage() {
        if (this.sendTo === "" || !this.sendTo || this.sendTo === "All")
            this.hubConnection
                .invoke('sendToAll', this.nick, this.message)
                .catch(err => console.error(err));
        else
            this.hubConnection.invoke('sendToClient', this.sendTo, this.message)
                .catch(err => console.error(err));
    }

    private upMessage = (nick: string, message: string, oneToOne = false) => {
        nick = oneToOne ? nick + " sent to you" : nick;
        const text = `${nick}: ${message}`;
        this.messages.push(text);
    }

    configureHubBehaviour() {

        this.hubConnection.on('sendToAll', (nick: string, message: string) => {
            this.upMessage(nick, message);
        });

        this.hubConnection.on('sendToClient', (nick: string, message: string) => {
            this.upMessage(nick, message, true);
        });

        this.hubConnection.on('getConnectedUsers', (info: ClientInfo[]) => {
            this.users = info;
        });
    }

}


export class ClientInfo {
    nickname: string;
    connectionId: string;
}