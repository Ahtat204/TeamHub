import { Component } from '@angular/core';
import {ProjectsStats} from '../projects-stats/projects-stats';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',

  imports: [
    ProjectsStats

  ]
})
export class Dashboard {
  bgColor = 'navy';
  textColor = 'white';
}
