using Tracker.Application;
using Tracker.Database;
using Tracker.Domain.Options;
using Tracker.Infrastructure;
using Tracker.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

DbMigrations.Initialize(connectionString);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
