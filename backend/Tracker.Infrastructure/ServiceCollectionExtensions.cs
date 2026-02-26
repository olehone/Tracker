using Azure.Storage.Blobs;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Jobs;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.States;
using Tracker.Domain.Options;
using Tracker.Infrastructure.Auth;
using Tracker.Infrastructure.Hagnfire;
using Tracker.Infrastructure.Redis;
using Tracker.Infrastructure.Services;

namespace Tracker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        AddJwtAuth(services);
        AddBlobStorage(services);
        AddRedis(services);
        AddHangfire(services);

        return services;
    }

    private static void AddJwtAuth(IServiceCollection services)
    {
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName);

        services.AddOptions<PasswordHasherOptions>()
            .BindConfiguration(PasswordHasherOptions.SectionName);

        services.AddHttpContextAccessor();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IUserContext, UserContext>();
    }

    private static void AddBlobStorage(IServiceCollection services)
    {
        services.AddOptions<BlobOptions>()
            .BindConfiguration(BlobOptions.SectionName);

        services.AddScoped((serviceProvider) =>
        {
            var blobOptions = serviceProvider.GetRequiredService<IOptions<BlobOptions>>().Value;
            return new BlobServiceClient(blobOptions.DefaultConnectionString);
        });
        services.AddScoped<IAvatarStorageService, AzureBlobAvatarStorageService>();
        services.AddScoped<IAttachmentStorageService, AzureBlobAttachmentStorageService>();
    }

    private static void AddRedis(IServiceCollection services)
    {
        services.AddOptions<RedisOptions>()
            .BindConfiguration(RedisOptions.SectionName);

        services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
        {
            var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>().Value;
            return ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
        });

        services.AddScoped<ICallState, RedisCallState>();
        services.AddScoped<IBoardCallState, RedisBoardCallState>();
    }

    public static IServiceCollection AddHangfire(this IServiceCollection services)
    {
        services.AddOptions<HangfireOptions>()
            .BindConfiguration(HangfireOptions.SectionName);

        services.AddHangfire((sp, cfg) =>
        {
            var options = sp.GetRequiredService<IOptions<DbOptions>>().Value;

            cfg.UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UseSqlServerStorage(options.DefaultConnectionString);
        });

        services.AddHangfireServer();
        services.AddScoped<IBoardArchivingJob, BoardArchivingJob>();
        
        return services;
    }
}