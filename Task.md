TASK TRACKER APP
(use Trello for reference)
1. Setup WebAPI solution with clean architecture. Consists of following layers:
 - Domain – entities, DTOs, constants, options etc.
 - Application – defines business logic – contains command and queries according to CQRS and key component abstractions (services and repos),
 - Infrastructure – implementation of services such as auth, file storage services etc.
 - Persistence – typically is used to extract DAL from Infrastructure layer, holds repo implementations and UOW.
 - Presentation – users entry-point app – WebAPI, (console/web app).
 - Database – not related to clean architecture, should contain migration scripts and BD initializer of DbUp library (REQUIRED – use DbUp scripts for migrations. EF of Dapper for data access).
2. Design database structure with ER diagram and extract (or write manually) create scripts for data tables.
3. Implement DAL with repository pattern (EF/Dapper).
Create base IRepository<Tentity,Tid> with default CRUD methods implementation (SimpleCrud for Dapper).
4. Implement commands and queries for data manipulation with MediatR package.
Queries (and sometimes commands) should not expose entities, use DTOs mapping. Implement API endpoints and add protect app with role-based auth (Azure EntraID will be a plus).
5. Create a frontend Blazor app, use any components library – Telerik, AntDesign, MudBlazor, SyncFunsion etc. Implement role-based auth. Use Refit package to make API calls to backend. Project layers: WebApp (as Presentation), Services, Services.Abstraction and Domain
6. Implement Azure BLOB file storage for user avatars and task attachments/images. Windows emulator available.
7. Implement archivation logic for projects.
 - Mark project/board as archived,
 - pick archived boards with HangFire recurrent job and place then to Azure ServiceBus queue,
 - create separate solution for with Azure Function app,
 - pick boards with Azure Function triggered on ServiceBus, serialize board/project with it’s info into a file and store to BLOB storage,
 - setup BLOB storage lifecycle management rules to move files to cooler tier after some time,
 - create Azure CosmosDB to store and track info about archivation job statuses and/or timelines. Windows emulator available.
8. AI chat window for FAQ - implement RAG pipeline with specific sensitive info – dummy questions and answers can be generated just to show functionality, using Azure Open AI and Azure AI Search with KernelMemory library (or SemanticKernel)
https://microsoft.github.io/kernel-memory/, https://www.developerscantina.com/p/kernelmemory/, https://www.developerscantina.com/p/semantic-kernel-memory/,
9. WebRTC for calls (with screen sharing feature),
10. SignalR for tracking real-time changes on board or inside of task,
11. Paid subscriptions with Stripe
