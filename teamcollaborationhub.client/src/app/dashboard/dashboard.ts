import {Component, Injectable} from '@angular/core';
import {ProjectsStats} from '../projects-stats/projects-stats';
import {Project, ProjectElement} from '../project-grid/project-element.component';
import {NgForOf} from '@angular/common';





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
  projects: Project[] = [
    new Project(
      1,
      "Completed",
      "AI Recommendation Engine",
      new Date("2025-06-30"),
      new Date("2025-02-01"),
      "Built a hybrid recommendation system combining collaborative and content-based filtering.",
      []
    ),
    new Project(
      2,
      "In Progress",
      "Real-Time Payment Gateway",
      new Date("2026-01-15"),
      new Date("2025-09-01"),
      "Microservices-based payment processing system with rate limiting and fraud detection."
    ),
    new Project(
      3,
      "Planned",
      "Kubernetes Deployment Platform",
      new Date("2026-05-01"),
      new Date("2026-02-01"),
      "CI/CD automation platform using container orchestration and GitOps principles."
    ),
    new Project(
      4,
      "Completed",
      "Computer Vision Attendance System",
      new Date("2025-04-20"),
      new Date("2024-11-10"),
      "Facial recognition-based attendance tracking system for educational institutions."
    ),
    new Project(
      5,
      "On Hold",
      "Game Physics Engine Prototype",
      new Date("2026-08-01"),
      new Date("2026-03-01"),
      "Experimental rigid-body physics engine optimized for multiplayer environments."
    )
  ];
}
