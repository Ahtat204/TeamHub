import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './user-login/user-login';

const routes: Routes = [{ path: '', redirectTo:"user-login",pathMatch:"full" },{path:"user-login",component:LoginComponent}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
