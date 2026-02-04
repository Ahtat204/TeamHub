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
    MatSuffix
  ],
  providers: [
    provideBrowserGlobalErrorListeners()
  ],
  bootstrap: [App]
})
export class AppModule { }
