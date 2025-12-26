using Tracker.API;
using Tracker.Application;
using Tracker.Database;
using Tracker.Infrastructure;
using Tracker.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["DbOptions:DefaultConnectionString"]!;
DbMigrations.Initialize(connectionString);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddJwtBearerAndAuth();
builder.Services.AddSwaggerAuth();

builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("DevCorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
