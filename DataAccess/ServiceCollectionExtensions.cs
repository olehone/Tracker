using ArchivingFunction.Domain.Options;
using ArchivingFunction.Persistence;
using Azure.Storage.Blobs;
using DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services)
    {
        AddAzureBlob(services);
        AddDatabase(services);

        return services;
    }

    private static void AddDatabase(IServiceCollection services)
    {
        services.AddOptions<DbOptions>()
                    .BindConfiguration(DbOptions.SectionName);

        services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DbOptions>>().Value;
            optionsBuilder.UseSqlServer(options.ConnectionString);
        });

        services.AddScoped<IBoardRepository, BoardRepository>();
    }

    private static void AddAzureBlob(IServiceCollection services)
    {
        services.AddOptions<BlobOptions>()
                    .BindConfiguration(BlobOptions.SectionName);

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BlobOptions>>().Value;
            return new BlobServiceClient(options.ConnectionString);
        });
    }
}