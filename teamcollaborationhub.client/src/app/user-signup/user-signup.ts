import {Component, Input} from '@angular/core';
import {HttpClient} from '@angular/common/http';



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


export class UserSignup {
   email=""
   password=""
   username=""


constructor(private http: HttpClient) {

}
public Url:string = 'http://localhost:8080';

public signup() {
  const request={"username":this.username,"email":this.email,password:this.password}
 const result=this.http.post(this.Url+'/signup',request);
  console.log(result);
}
}
