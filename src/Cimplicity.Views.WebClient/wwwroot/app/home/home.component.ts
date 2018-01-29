import { Component, OnInit } from '@angular/core';
//import { HubConnection } from "@aspnet/signalr-client";

@Component({
    selector: 'home',
    template: `
        <h1>Home</h1>
        <p>Hello you !</p>
        <ul>
        <li *ngFor="let m of messages">m</li>
        </ul>
        <button (click)="sendMessage">Send!</button>
    `
})
export class HomeComponent implements OnInit {

    //private _hubConnection: HubConnection;
    //message = '';
    //messages: string[] = [];

    constructor() {
        console.log('HomeComponent -> constructor');

    }



    ngOnInit() {
        console.log('HomeComponent -> ngOnInit');
    //    this._hubConnection = new HubConnection("/testHub");

    //    this._hubConnection.on('Send', (data: any) => {
    //        const received = `Received: ${data}`;
    //        this.messages.push(received);
    //    });

    //    this._hubConnection.start()
    //        .then(() => {
    //            console.log('Hub connection started')
    //        })
    //        .catch(err => {
    //            console.log('Error while establishing connection')
    //        });
   }
    //public sendMessage(): void {
    //    const data = `Sent: ${this.message}`;

    //    this._hubConnection.invoke('Send', data);
    //    this.messages.push(data);
    //}
}