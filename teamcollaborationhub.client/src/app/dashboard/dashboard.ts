import { Component } from '@angular/core';
import {ProjectsStats} from '../projects-stats/projects-stats';
import {ProjectGrid} from '../project-grid/project-grid';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',

  imports: [
    ProjectsStats,
    ProjectGrid

  ]
})
export class Dashboard {
  bgColor = 'navy';
  textColor = 'white';
}
