import {Component} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {CommonModule} from "@angular/common";
import {MatCardModule} from "@angular/material/card";
import {MatInputModule} from "@angular/material/input";
import {MatButtonModule} from "@angular/material/button";
import {MatIconModule} from "@angular/material/icon"
import {MatFormFieldModule} from "@angular/material/form-field"

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, MatCardModule, MatInputModule, MatButtonModule, MatIconModule, MatFormFieldModule],
  templateUrl: './user-login.html',
  styleUrl: './user-login.css'
})
export class LoginComponent {
user={
  email:"",
  password:""
}
}
