import { Component } from '@angular/core';
import {NgForOf} from '@angular/common';

interface projectsStatus {
  projectsCount: number;
  ProjectStatus: string;
}

@Component({
  selector: 'app-projects-stats',
  standalone: true,
  templateUrl: './projects-stats.html',
  styleUrl: './projects-stats.css',
  imports: [
    NgForOf
  ]
})
export class ProjectsStats {
projectstats:projectsStatus[]=[]
  ngOnInit() {
  this.projectstats=[
    {projectsCount:0,ProjectStatus:'inProgress'},
    {projectsCount:20,ProjectStatus:'outProgress'},
    {projectsCount:30,ProjectStatus:'outProgress'},
    {projectsCount:50,ProjectStatus:'outProgress'},
  ];
  }
}
