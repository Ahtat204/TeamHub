import {Component, Inject, Injectable, Input} from '@angular/core';
import {MatCardModule} from '@angular/material/card';
import {User} from '../user-signup/user-signup';


export class Project {
  public Id: Number;
  public Name: string;
  public Description: string;
  public Contributors?: User[];
   public Status: string;
  public EndDate: Date;
  public StartDate: Date;


  constructor(Id: Number,Status: string, Name: string, EndDate: Date, StartDate: Date, Description: string, Contributors?: User[]) {
    this.Id = Id;
    this.Name = Name;
    this.Description = Description;
    this.Contributors = Contributors;
    this.Status = Status;
    this.EndDate = EndDate;
    this.StartDate = StartDate;
  }
}

@Component({
  selector: 'app-project-grid',
  standalone: true,
  imports: [MatCardModule],
  templateUrl: './project-element.component.html',
  styleUrl: './project-element.component.css',
})
export class ProjectElement {
  @Input() public project!: Project;

}
