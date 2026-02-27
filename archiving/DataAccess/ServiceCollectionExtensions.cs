using Azure.Storage.Blobs;
using DataAccess.Abstractions;
using Domain.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services, bool isLocal = false)
    {
        AddAzureBlob(services);
        AddDatabase(services);
        AddCosmosDb(services, isLocal);

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

        services.AddScoped<IKeyStringStorage, BlobKeyStringStorage>();
    }

    private static void AddCosmosDb(IServiceCollection services, bool isLocal)
    {
        services.AddOptions<CosmosDbOptions>()
                    .BindConfiguration(CosmosDbOptions.SectionName);

        services.AddOptions<CosmosDbOptions>()
               .BindConfiguration(CosmosDbOptions.SectionName);

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
            var clientOptions = new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };

            if (isLocal)
            {
                clientOptions.HttpClientFactory = () => new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
                clientOptions.ConnectionMode = ConnectionMode.Gateway;
            }

            return new CosmosClient(options.ConnectionString, clientOptions);
        });

        services.AddScoped<IBoardMetadataStorage, CosmosDbBoardMetadataStorage>();
    }
}