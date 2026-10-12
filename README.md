# ChoreApp

A chore-tracking app built around streaks and XP, in the spirit of Duolingo. It's designed **roommates-first**: peer-based, with no built-in parent or admin authority. A family mode is planned as an optional layer on top.

> **Status: work in progress.** The backend API and data model are working. The MAUI client is scaffolded and running, and wiring it to the API is the current step. See [Roadmap](#roadmap) for what's done and what isn't.

## What it does

- Households, users and chores, managed through a REST API
- **Mandatory chores** (assigned to a specific user) and **bonus chores** (open to anyone, worth XP to whoever completes them)
- Completing a chore awards its XP to the user who completed it; for bonus chores, the completer becomes the assignee
- Data persisted in SQLite through EF Core migrations

## Tech stack

| Part | Technology |
| --- | --- |
| Backend | ASP.NET Core Minimal API (.NET 10) |
| Data access | Entity Framework Core + SQLite |
| Client | .NET MAUI (targets Windows and Android) |
| Language | C# throughout |

## Project structure

```
ChoreApp/
├── ChoreApp.slnx
├── Server/    ASP.NET Core Minimal API, EF Core models, migrations
└── App/       .NET MAUI client (in progress)
```

The server and the app are separate projects with separate `.csproj` files, and they deliberately do not share model classes. The server's entities describe how data is stored, while the client only needs the shape of what the API returns, and keeping them apart means server-only fields never leak into the client.

## Getting started

**Prerequisite:** [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
cd Server
dotnet run
```

Migrations are applied automatically on startup and the SQLite database file is created in the `Server` folder. The console prints the URL the API is listening on.

## API

| Method | Route | Description |
| --- | --- | --- |
| GET | `/households` | List households |
| POST | `/households` | Create a household |
| PATCH | `/households/{id}` | Partially update a household |
| GET | `/users` | List users |
| POST | `/users` | Create a user |
| PATCH | `/users/{id}` | Partially update a user |
| GET | `/chores` | List chores |
| GET | `/chores/{id}` | Get one chore |
| POST | `/chores` | Create a chore |
| PATCH | `/chores/{id}` | Partially update a chore |
| PATCH | `/chores/{id}/completion` | Complete a chore and award XP |
| DELETE | `/chores/{id}` | Delete a chore |

### Example requests

Replace `<port>` with the port printed by `dotnet run`.

```bash
# Create a household
curl -X POST http://localhost:<port>/households \
  -H "Content-Type: application/json" \
  -d '{"name": "Flat 4B"}'

# Create a user in it
curl -X POST http://localhost:<port>/users \
  -H "Content-Type: application/json" \
  -d '{"name": "Alex", "householdId": 1}'

# Create a bonus chore (no assignee, anyone can complete it)
curl -X POST http://localhost:<port>/chores \
  -H "Content-Type: application/json" \
  -d '{"name": "Clean the oven", "description": "Inside and racks", "isMandatory": false, "xpReward": 50, "householdId": 1}'

# Alex completes it; the server marks it done and awards the XP
curl -X PATCH http://localhost:<port>/chores/1/completion \
  -H "Content-Type: application/json" \
  -d '{"assignedUserId": 1}'
```

## Data model

- **Household**: `Id`, `Name`
- **User**: `Id`, `Name`, `Xp`, `Streak`, `LastChoreDayCompletedAt`, `HouseholdId` (nullable)
- **Chore**: `Id`, `HouseholdId`, `AssignedUserId` (nullable), `Name`, `Description`, `IsMandatory`, `IsComplete`, `CompletedAt` (nullable), `XpReward`

## Design decisions

- **Foreign keys on the "many" side only.** There are no stored lists of IDs on parent entities. A user's chores are found by querying `AssignedUserId`, so there is a single source of truth.
- **Deliberate delete behavior.** Deleting a household cascades to its chores, but its users are kept with `HouseholdId` set to null, so an account can outlive any one household. Deleting a user sets their chores' `AssignedUserId` to null instead of deleting them.
- **Server-assigned integer IDs.** Clients never choose an ID; the response to a POST returns the created object with its ID.
- **Dedicated completion endpoint.** Completing a chore is one specific action with side effects (XP, timestamp, assignee), not a generic field edit. It takes a single-field DTO, and the server sets `IsComplete` and `CompletedAt` itself rather than trusting the client's clock.
- **Partial updates use DTOs with nullable fields**, so a field that wasn't sent is distinguishable from one explicitly set to `false`.
- **No premature features.** The data model was kept minimal on purpose (no completion-history table, no recurrence, int IDs instead of GUIDs) and will grow once real usage shows what's needed.

## Known limitations

- **No authentication yet.** The client tells the server which user it is. That is acceptable for a local, single-household setup and will need real auth before the app is ever exposed publicly.
- Create endpoints currently accept the full entity; dedicated create DTOs are planned.

## Roadmap

- [x] Data model and EF Core migrations (SQLite)
- [x] CRUD endpoints for households, users and chores
- [x] Chore completion with XP
- [x] MAUI project scaffolded and running as a Windows desktop app
- [ ] Connect the MAUI client to the API
- [ ] Streak logic (calculated lazily from the last completed day, no scheduled job)
- [ ] Peer-based veto system for false completions
- [ ] Push notifications with funny reminders
- [ ] Authentication, then multi-household support