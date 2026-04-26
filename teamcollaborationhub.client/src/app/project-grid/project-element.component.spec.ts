import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectElement } from './project-element.component';

describe('ProjectGrid', () => {
  let component: ProjectElement;
  let fixture: ComponentFixture<ProjectElement>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProjectElement]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectElement);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
