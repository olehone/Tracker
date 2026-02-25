using Tracker.API;
using Tracker.API.Hubs;
using Tracker.Application;
using Tracker.Database;
using Tracker.Domain.Options;
using Tracker.Infrastructure;
using Tracker.Persistence;

var builder = WebApplication.CreateBuilder(args);

var dbOptions = builder.Configuration.GetSection("DbOptions").Get<DbOptions>()!;
DbMigrations.Initialize(dbOptions.DefaultConnectionString);

var corsOptions = builder.Configuration.GetSection("CorsOptions").Get<CorsOptions>()!;

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy.WithOrigins(corsOptions.AllowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var blobOptions = builder.Configuration.GetSection("BlobOptions").Get<BlobOptions>()!;

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = blobOptions.ItemAttachmentMaxSize;
});

builder.Services.AddControllers();
builder.Services.AddJwtBearerAndAuth();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerAuth();
}

builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices();
builder.Services.AddSignalR(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors = true;
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("DevCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<BoardHub>("/hubs/board")
   .RequireCors("DevCorsPolicy");
app.MapHub<ItemHub>("/hubs/item")
   .RequireCors("DevCorsPolicy");
app.MapHub<CallHub>("/hubs/call")
   .RequireCors("DevCorsPolicy");

await app.RunAsync();