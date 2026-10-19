# SEN381_CivicConnect

A community service-request management platform built for SEN381. CivicConnect replaces a fragmented mix of email, WhatsApp, spreadsheets and paper-based tracking with a single controlled, traceable digital record for every service request, from submission through to resolution.

**Final submission:** commit `94fe060de28821e96a2d0b61b4c314390e79b755` (PED v4.0 Final).

## Purpose

Community organisations often manage facility faults, IT support requests, maintenance issues and other operational requests through informal, disconnected channels. This leads to duplicated or lost requests, unclear ownership, poor visibility for requesters, and unreliable reporting for management. CivicConnect provides:

- **Requesters**: submit, categorise and track service requests, with status visibility and feedback from submission to closure.
- **Staff**: a controlled view of the requests in their categories, with tools to accept, assign, start, resolve, close and annotate requests.
- **Management**: a read-only overview of requests by status, staff activity, and a filterable list of all requests.

## Final Status (Milestone 4)

> The Project Evidence Document (PED v4.0 Final) holds the full requirements baseline, traceability matrix, risk, defect and debt registers, and decision log.

| Area | Status |
|---|---|
| Requirements baseline (FR-001 to FR-016, NFR-001 to NFR-010) | Complete (M1) |
| Architecture and technology-stack decision | Complete (M2) |
| Core system: submission, workflow, audit, reporting | Implemented (M3) |
| Automated tests | 92 tests (52 unit, 23 black-box, 15 integration, 2 end-to-end), run in CI |
| CI pipeline (build, format check, tests, dependency audit, CodeQL) | In place |
| Scope completion | 86.5% (22.5 of 26 acceptance criteria) |
| Hosted deployment | **Not performed. Declared staging no-go for production** (see PED Section 31) |

Known partial items: FR-014 (overdue reporting, deferred by CC-002) and FR-016 (quality metrics, limited by CC-003).

## Technology Stack

- **Backend:** ASP.NET Core 8 (C#), MVC with Razor views and Bootstrap
- **Persistence:** Entity Framework Core 8 + SQL Server (LocalDB for development)
- **Auth:** cookie authentication with the framework `PasswordHasher` (PBKDF2, 220,000 iterations), our own `User` table with a role column, and policy-based authorisation
- **Tests:** xUnit, with the EF Core in-memory provider in CI
- **CI:** GitHub Actions (`build-test`, `dependency-audit`), CodeQL, Dependabot

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB (installed with Visual Studio) or SQL Server Express
- The EF Core tool: `dotnet tool install --global dotnet-ef`

## Setup and Run (Development)

Run these in PowerShell:

```powershell
git clone https://github.com/AidanSm2103/SEN381_CivicConnect.git
cd SEN381_CivicConnect\src\CivicConnect

# One-time: password used for the demo accounts (never committed)
dotnet user-secrets set "SeedPassword" "<choose a password>"

dotnet run
```

Open the `https://localhost:xxxx` address printed in the console. In the Development environment the app applies migrations and seeds demo data on start, so no separate `dotnet ef database update` step is needed.

Demo accounts (all use the `SeedPassword` you set):

| Role | Email |
|---|---|
| Requester | `requester@civicconnect.test` |
| Staff | `staff@civicconnect.test`, `staff2@civicconnect.test` |
| Management | `manager@civicconnect.test` |

You can also register a new account from the site; self-registration only creates Requesters.

> If a database was created before the M3 migration, drop it first (`dotnet ef database drop`). The old schema can fail the migration (known limitation, PED Section 32).

## Run the Tests

```powershell
cd SEN381_CivicConnect
dotnet test src/CivicConnect/CivicConnect.sln --configuration Release --filter "Category!=Performance"
```

The performance test runs against a live instance and is skipped unless `PERF_BASE_URL`, `PERF_EMAIL` and `PERF_PASSWORD` are set.

## Environments

| Environment | Behaviour |
|---|---|
| Development | Applies migrations and seeds demo users |
| Staging | Applies migrations; seeds demo users only if `SeedDemoData=true` |
| Production | Applies migrations only; never seeds |

To run a Production-style check locally, start the app with `dotnet run --no-launch-profile --configuration Release` and set `ASPNETCORE_ENVIRONMENT=Production` plus the connection string as environment variables. Plain `dotnet run` uses `launchSettings.json`, which forces Development.

## Repository Structure

```
/
├── .github/               # CI workflow, CodeQL, Dependabot, CODEOWNERS
├── Documentation/         # Ownership and branching plan, supporting documents
├── src/
│   ├── CivicConnect/      # Web application (Controllers, Services, Repositories, Data, Views)
│   └── CivicConnect.Tests/# Unit, black-box, integration, end-to-end and performance tests
├── .gitignore
├── LICENSE
└── README.md
```

The code is layered: Controllers, then Services, then Repositories, then the `DbContext`. Services return `Result` objects, and authorisation is enforced at both the controller (policies) and service level.

## Configuration and Secrets

No secrets are committed. `appsettings.json` holds only the LocalDB connection string. The demo seed password comes from `dotnet user-secrets` (key `SeedPassword`). Staging and production settings are supplied as environment variables or hosting application settings.

## Contributing / Workflow

- `main` is protected: every change enters through a Pull Request.
- Every PR needs two approvals from team members other than the author, and the required checks `build-test` and `dependency-audit` must pass on an up-to-date branch.
- Branches are short-lived and named by purpose (`feature/`, `test/`, `chore/`, `release/`).
- Files marked as frozen contracts change only through a "CONTRACT CHANGE" PR.

## Known Limitations

- No login lockout or rate limiting, password reset or email verification (TD-002). This is the main blocker for production use.
- No hosted deployment; the app has only been run locally. Hosted TLS and availability (NFR-005) are untested.
- CI uses the in-memory database, so SQL Server behaviour (CHECK constraint, RowVersion, transactions) is verified by recorded manual checks on LocalDB.
- Overdue reporting (FR-014) and staff quality metrics (FR-016) are partial by approved change.
- Usability (NFR-007) and cross-browser (NFR-006) testing are incomplete.
- Backups are manual; the restore procedure was proven locally.

The full list with future work is in PED Section 32.

## Project Documentation

Requirements, architecture, risk register, traceability matrix, defect and technical-debt registers, and the engineering decision log are in the Project Evidence Document (PED v4.0 Final), submitted separately.

## Team

- Aidan Smith (601819)
- Enrique de Sousa (603110)
- Michael-John Robinson (602310)

## License

This project is licensed under the MIT License. See [LICENSE](./LICENSE) for details.

