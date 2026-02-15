<pre>
TeamcollborationHub.server/
│
├── appsettings.json
├── appsettings.Development.json
├── docker-compose.yml
├── Dockerfile
├── Program.cs
├── TeamcollborationHub.server.csproj
├── TeamcollborationHub.server.sln
│
├── Configuration/
│   └── TDBContext.cs
│
├── Controllers/
│   └── AuthenticationController.cs
│
├── Dto/
│   └── UserDto.cs
│
├── Entities/
│   ├── Comment.cs
│   ├── Project.cs
│   ├── ProjectTask.cs
│   └── User.cs
│
├── Enums/
│   └── ProjectStatus.cs
│
├── Exceptions/
│   └── NotFoundException.cs
│
├── Features/
│   └── Projects/
│       ├── Commands/
│       │   ├── AddContributorToProject/
│       │   │   └── AddContributorToProjectCommand.cs
│       │   ├── AddProjectTask/
│       │   │   └── AddProjectTask.cs
│       │   ├── CreateProject/
│       │   │   ├── CreateProjectCommand.cs
│       │   │   └── CreateProjectCommandHandler.cs
│       │   ├── RemoveContributorFromProject/
│       │   │   └── RemoveContributorFromProject.cs
│       │   ├── RemoveProjectTask/
│       │   │   └── RemoveProjectTask.cs
│       │   ├── SetProjectDeadline/
│       │   │   └── SetProjectDeadline.cs
│       │   └── SetProjectStartDate/
│       │       └── SetProjectStartDate.cs
│       │
│       └── Queries/
│           ├── GetAllProjectContributors/
│           │   ├── GetAllProjectContributorsQuery.cs
│           │   └── GetAllProjectContributorsQueryHandler.cs
│           ├── GetAllProjectsQuery/
│           │   ├── GetAllProjectsQuery.cs
│           │   └── GetAllProjectsQueryHandler.cs
│           ├── GetAllProjectTasks/
│           │   ├── GetAllProjectTasksQuery.cs
│           │   └── GetAllProjectTasksQueryHandler.cs
│           ├── GetProjectById/
│           │   ├── GetProjectByIdQuery.cs
│           │   └── GetProjectByIdQueryHandler.cs
│           ├── GetProjectContributorsById/
│           │   ├── GetProjectContributorsByIdQuery.cs
│           │   └── GetProjectContributorsByIdQueryHandler.cs
│           └── GetProjectTaskById/
│               ├── GetProjectTaskByIdQuery.cs
│               └── GetProjectTaskByIdQueryHandler.cs
│
├── Migrations/
│   ├── 20251028201815_first migration.cs
│   ├── 20251028204033_enum-string conversion.cs
│   ├── 20251205143106_UserUpdated.cs
│   ├── 20251205170840_Delete-ProjectId.cs
│   ├── 20251205172052_ProjectIdForeignKey.cs
│   └── TDBContextModelSnapshot.cs
│
├── Procedures/
│   ├── CompletedProjectsPercentage.sql
│   ├── CompletedTasksPerProject.sql
│   ├── ContributorsWithNoActiveProjects.sql
│   ├── GetProjectByContributor.sql
│   ├── NumberOfProjectsPerStatus.sql
│   ├── ProjectsCreatedPerDayWeek.sql
│   ├── ProjectsCreatedPerMonth.sql
│   ├── TaskCompletionRateAcrossAllProjects.sql
│   ├── TasksCompletedPerPeriod.sql
│   ├── TopContributorsByActivity.sql
│   └── VelocityMetrics.sql
│
├── Properties/
│   └── launchSettings.json
│
├── Repositories/
│   └── UserRepository/
│       ├── IUserRepository.cs
│       └── UserRepository.cs
│
└── Services/
├── Authentication/
│   ├── Jwt/
│   │   ├── IJwtService.cs
│   │   └── JwtService.cs
│   └── UserAuthentication/
│       ├── IAuthenticationService.cs
│       └── AuthenticationService.cs
│
├── Caching/
│   ├── ICachingService.cs
│   └── RedisCachingService.cs
│
├── Projects/
│   ├── IProjectService.cs
│   └── ProjectService.cs
│
└── Security/
├── IPasswordHashingService.cs
└── PasswordHashing.cs
</pre>