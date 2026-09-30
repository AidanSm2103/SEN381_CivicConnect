# SEN381_CivicConnect

A community service-request management platform built for SEN381. CivicConnect replaces a fragmented mix of email, WhatsApp, spreadsheets and paper-based tracking with a single controlled, traceable digital record for every service request — from submission through to resolution.

## Purpose

Community organisations often manage facility faults, IT support requests, maintenance issues, lost property and other operational requests through informal, disconnected channels. This leads to duplicated or lost requests, unclear ownership, poor visibility for requesters, and unreliable reporting for management. CivicConnect provides:

- **Requesters** — a simple way to submit, categorise and track service requests, with status visibility from submission to closure.
- **Staff** — a controlled view of requests relevant to their role, with tools to assign, update, resolve and record actions.
- **Management** — oversight of service activity, request status, and accountability/performance information.

## Current Implementation Status

> See the PED for the full requirements baseline, architecture decisions and traceability matrix.

| Area | Status |
|---|---|
| Requirements baseline (FR-001–016, NFR-001–010) | Complete (M1) |
| Architecture & technology-stack decision | Complete (M2) |
| Data model / persistence | In progress |
| Core request-submission flow (FR-001) | In progress |
| Staff/Management views | Planned |
| Automated testing | Planned |
| CI pipeline | Planned |
| Deployment (Azure App Service) | Planned |

## Technology Stack

- **Backend:** ASP.NET Core (C#), .NET 8 LTS
- **Frontend:** Razor Pages / MVC
- **Persistence:** Entity Framework Core + SQL Server
- **Auth:** ASP.NET Core Identity with policy-based, role-based access control
- **Hosting (planned):** Azure App Service, F1 (free) tier

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express / LocalDB, or SQLite (for local development)
- Visual Studio 2022 (Community Edition is free) or VS Code with the C# extension

## Setup & Run

```bash
# Clone the repository
git clone https://github.com/AidanSm2103/SEN381_CivicConnect.git
cd SEN381_CivicConnect

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the application
dotnet run
```

> Update this section with the actual project path and any environment configuration once the solution structure is finalised.

## Repository Structure

```
/
├── Documentation/         # PED, requirements, architecture, decisions, risk, change, quality, security, deployment
├── src/                   # Application source code
├── tests/                 # Automated tests
├── .github/
│   ├── workflows/         # CI configuration
│   └── pull_request_template.md
├── .gitignore
├── LICENSE
└── README.md
```

## Configuration & Secrets

Connection strings and credentials are kept out of source control and managed via environment-based configuration

## Contributing / Collaboration Workflow

- `main` is protected — all changes enter via Pull Request.
- Every PR requires **two approvals** from team members other than the author; self-approval is disabled.
- Branch per issue/feature, short-lived, scoped to one person's workstream.

## Known Limitations / TODOs

- Automated test coverage not yet in place.
- CI pipeline not yet configured (planned for M3).
- Production deployment not yet performed; current hosting evidence is research-based, not deployment-tested.

## Project Documentation

Full requirements, architecture decisions, risk register, traceability matrix and engineering decision log are maintained in the Project Engineering Document (PED).

## Team

- Aidan Smith
- Enrique de Sousa
- Michael-John Robinson

## License

This project is licensed under the MIT License — see [LICENSE](./LICENSE) for details.
