import {Component, Input} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import {User} from '../user-signup/user-signup';
import {Project} from '../dashboard/dashboard';


@Component({
  selector: 'app-project-grid',
  standalone: true,
  imports: [MatButton, MatCardModule],
  templateUrl: './project-element.component.html',
  styleUrl: './project-element.component.css',
})
export class ProjectElement {
  @Input() public project:Project;
  constructor(project: Project) {
    this.project = project;
  }
}
