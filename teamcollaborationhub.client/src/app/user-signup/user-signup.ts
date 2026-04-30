import {Component, Input, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiClient} from '../api-client';


export class User{
public name: string;
public email: string;
public password: string;
constructor(name: string, email: string, password: string) {
  this.name = name;
  this.email = email;
  this.password = password;
}
}

@Component({
  selector: 'app-user-signup',
  standalone: false,
  templateUrl: './user-signup.html',
  styleUrl: './user-signup.css',

})


export class UserSignup  {
   email=""
   password=""
   username=""


  constructor(private apiClient: ApiClient) {}

  signupButton() {
    const result=this.apiClient.signUp(this.username,this.email,this.password);
    result.subscribe()
  }
}
