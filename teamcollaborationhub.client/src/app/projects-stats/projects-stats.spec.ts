import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectsStats } from './projects-stats';

describe('ProjectsStats', () => {
  let component: ProjectsStats;
  let fixture: ComponentFixture<ProjectsStats>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProjectsStats]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectsStats);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
