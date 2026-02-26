using ArchivingFunction;
using ArchivingFunction.Domain.Options;
using ArchivingFunction.Interfaces;
using ArchivingFunction.Persistence;

using Azure.Storage.Blobs;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddOptions<BlobOptions>()
    .BindConfiguration(BlobOptions.SectionName);

builder.Services.AddOptions<DbOptions>()
    .BindConfiguration(DbOptions.SectionName);

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<BlobOptions>>().Value;
    return new BlobServiceClient(options.ConnectionString);
});

builder.Services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<DbOptions>>().Value;
    optionsBuilder.UseSqlServer(options.ConnectionString);
});

builder.Services.AddScoped<IBoardRepository, BoardRepository>();

builder.Build().Run();
