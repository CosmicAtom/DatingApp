import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.huburl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUser$ = this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService, private router : Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

    this.hubConnection
      .start()
      .catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      //this.toastr.info(username + ' has connected');
      this.onlineUser$.pipe(take(1)).subscribe(usernames => {
        this.onlineUserSource.next([...usernames, username])
      })
    })

    this.hubConnection.on('UserIsOffline', username => {
      //this.toastr.error(username + ' has disconnected');
      this.onlineUser$.pipe(take(1)).subscribe(usernames => {
        this.onlineUserSource.next([...usernames.filter(x => x !== username)])
      })
    })

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUserSource.next(usernames);
    })

    this.hubConnection.on('NewMessageReceived', ({username, knownAs }) => {
      this.toastr.info( knownAs + ' has sent you a new message!')
      .onTap
      .pipe(take(1))
      .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'));
    })
  }

  stoHubConnection() {
    this.hubConnection.stop().catch(error => console.log(error));
  }
}
