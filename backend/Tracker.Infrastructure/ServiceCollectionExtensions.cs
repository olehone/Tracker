using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using StackExchange.Redis;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Jobs;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.States;
using Tracker.Domain.Options;
using Tracker.Infrastructure.Auth;
using Tracker.Infrastructure.AzureAI;
using Tracker.Infrastructure.Hagnfire;
using Tracker.Infrastructure.Redis;
using Tracker.Infrastructure.Services;
using Tracker.Infrastructure.Stripe;

namespace Tracker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        AddJwtAuth(services);
        AddBlobStorage(services);
        AddRedis(services);
        AddHangfire(services);
        AddServiceBus(services);
        AddStripe(services);
        services.AddScoped<IFaqService, FaqServiceMock>();

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

    public static void AddHangfire(IServiceCollection services)
    {
        services.AddOptions<HangfireOptions>()
            .BindConfiguration(HangfireOptions.SectionName);

        services.AddHangfire((serviceProvider, config) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DbOptions>>().Value;

            config.UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UseSqlServerStorage(options.DefaultConnectionString);
        });

        services.AddHangfireServer();
        services.AddScoped<IBoardArchivingJob, BoardArchivingJob>();
        services.AddScoped<IBoardUnarchivingJob, BoardUnarchivingJob>();
    }

    public static void AddServiceBus(IServiceCollection services)
    {
        services.AddOptions<ServiceBusOptions>()
            .BindConfiguration(ServiceBusOptions.SectionName);

        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
            return new ServiceBusClient(options.ConnectionString);
        });
    }

    public static void AddStripe(IServiceCollection services)
    {
        services.AddOptions<StripeOptions>()
            .BindConfiguration(StripeOptions.SectionName);
        services.AddScoped<IUserSubscriptionService, StripeService>();
    }


    public static void AddAzureAI(IServiceCollection services)
    {
        services.AddOptions<AIOptions>()
            .BindConfiguration(AIOptions.SectionName);

        services.AddSingleton<IKernelMemory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AIOptions>>().Value;

            return new KernelMemoryBuilder()
                .WithAzureOpenAITextGeneration(new AzureOpenAIConfig
                {
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    Endpoint = options.OpenAIEndpoint,
                    APIKey = options.OpenAIApiKey,
                    Deployment = options.Deployment
                })
                .WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
                {
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    Endpoint = options.OpenAIEndpoint,
                    APIKey = options.OpenAIApiKey,
                    Deployment = options.EmbeddingDeployment
                })
                .WithAzureAISearchMemoryDb(new AzureAISearchConfig
                {
                    Auth = AzureAISearchConfig.AuthTypes.APIKey,
                    Endpoint = options.AzureAISearchEndpoint,
                    APIKey = options.AzureAISearchApiKey
                })
                .Build<MemoryServerless>(new KernelMemoryBuilderBuildOptions
                {
                    AllowMixingVolatileAndPersistentData = true
                });
        });
        services.AddScoped<IFaqService, AzureAIFaqService>();
    }
}