import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';


@Injectable({
  providedIn: 'root',
})
export class ApiClient {
  constructor(private http: HttpClient) {
  }
  public Url:string = 'http://localhost:8080';

  signUp(username: string, email: string, password: string) {
    const request={"username":username,"email":email,password:password}
    const result=this.http.post(this.Url+'/signup',request);
    console.log(result);
    return result;
  }
}
