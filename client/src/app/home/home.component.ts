import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { every } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;

  constructor() {}
  
  ngOnInit() {
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  // getUsers(){
  //   this.http.get('https://localhost:7239/api/users').subscribe(users => this.users = users); 
  // }

  cancelRegisterMode(event : boolean){
    this.registerMode = event ;
  }
}
