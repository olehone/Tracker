## About 

This is a full-stack task management application, featuring Kanban boards, a roadmap view, calendar, and Eisenhower matrix for task prioritization. The backend is built with ASP.NET Core following clean architecture (Domain, Application, Infrastructure, Persistence, Presentation), using CQRS with MediatR, Dapper with DbUp migration scripts, and role-based authentication secured with JWT. The frontend is a Blazor WebAssembly app powered by MudBlazor, communicating with the API via Refit. The system integrates Azure Blob Storage for file attachments and user avatars, Azure Service Bus and Azure Functions for board archivation pipelines, CosmosDB for archivation job tracking, Redis for real-time call session management, and Hangfire for scheduled jobs. Additional features include an AI-powered FAQ chat built with a RAG pipeline using Azure OpenAI and Azure AI Search via KernelMemory, WebRTC-based calls with screen sharing, SignalR for live board updates, and Stripe for paid subscription management. Local development runs entirely in Docker using Azure emulators.

## Deployment

Docker and Git are required.

```bash
git clone https://github.com/olehone/Tracker
cd tracker/local
```

Copy the environment template and fill it in:

```bash
cp example.env .env
```

```env
SQL_SA_PASSWORD=

STRIPE_API_KEY=

DB_CONNECTION_STRING=
BLOB_CONNECTION_STRING=

JWT_SECRET_KEY=

STRIPE_WEBHOOK_SECRET=
STRIPE_SECRET_KEY=

OPENAI_ENDPOINT=
OPENAI_API_KEY=
AI_SEARCH_ENDPOINT=
AI_SEARCH_API_KEY=
```

Run:

```bash
docker compose up -d
```

By default the system uses local emulators: Azurite instead of Azure Blob Storage, Service Bus Emulator, and CosmosDB Emulator. Their configuration is already defined in `appsettings.Development.json` and requires no changes.

To use real cloud services instead of emulators, add the corresponding variables to `.env` — they will override the values from `appsettings.Development.json`:

```env
BlobOptions__DefaultConnectionString=
RedisOptions__ConnectionString=
ServiceBusOptions__ConnectionString=
CosmosOptions__ConnectionString=
```
