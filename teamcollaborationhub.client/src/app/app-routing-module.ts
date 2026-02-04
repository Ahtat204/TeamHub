import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './user-login/user-login';
import {UserSignup} from './user-signup/user-signup';

const routes: Routes = [{ path: '', redirectTo:"",pathMatch:"full" },{path:"login",component:LoginComponent},{path:'register',component:UserSignup}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
