import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './user-login/user-login';
import {UserSignup} from './user-signup/user-signup';
import {Dashboard} from './dashboard/dashboard';

const routes: Routes = [{ path: '', redirectTo:"",pathMatch:"full" },{path:"login",component:LoginComponent},{path:'register',component:UserSignup},{path:"home",component:Dashboard}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
