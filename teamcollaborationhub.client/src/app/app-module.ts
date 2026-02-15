import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { UserSignup } from './user-signup/user-signup';
import {FormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatCard, MatCardContent, MatCardHeader} from '@angular/material/card';
import {MatFormField, MatSuffix} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {MatInput} from '@angular/material/input';
import { Dashboard } from './dashboard/dashboard';
import { ProjectsStats } from './projects-stats/projects-stats';

@NgModule({
  declarations: [
    App,
    UserSignup
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    MatButton,
    MatCard,
    MatCardContent,
    MatCardHeader,
    MatFormField,
    MatIcon,
    MatInput,
    MatSuffix,
    Dashboard,
    ProjectsStats
  ],
  providers: [
    provideBrowserGlobalErrorListeners()
  ],
  exports: [
    ProjectsStats
  ],
  bootstrap: [App]
})
export class AppModule { }
