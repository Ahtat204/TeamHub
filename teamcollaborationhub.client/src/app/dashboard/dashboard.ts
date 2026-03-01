import { Component } from '@angular/core';
import {ProjectsStats} from '../projects-stats/projects-stats';
import {ProjectElement} from '../project-grid/project-element.component';
import {NgForOf} from '@angular/common';
import {User} from '../user-signup/user-signup';

export class Project{
  public Id:Number;
  public Name:string;
  public Description:string;
  public Contributors?:User[];
  public Status:string;
  public EndDate:Date;
  public StartDate:Date;
  constructor(Id:Number,Status:string,Name:string,EndDate:Date,StartDate:Date,Description:string,Contributors?:User[]) {
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
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',

  imports: [
    ProjectsStats,
    ProjectElement,
    NgForOf

  ]
})
export class Dashboard {
  projects:Project[]=new Array<Project>();
}
