using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Domain.Options;
using Tracker.Infrastructure.Auth;
using Tracker.Infrastructure.Services;

namespace Tracker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName);

        services.AddOptions<PasswordHasherOptions>()
            .BindConfiguration(PasswordHasherOptions.SectionName);

        services.AddHttpContextAccessor();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IUserContext, UserContext>();


        services.AddOptions<BlobOptions>()
            .BindConfiguration(BlobOptions.SectionName);

        services.AddScoped((serviceProvider) =>
        {
            var blobOptions = serviceProvider.GetRequiredService<IOptions<BlobOptions>>().Value;
            return new BlobServiceClient(blobOptions.DefaultConnectionString);
        });
        services.AddScoped<IAvatarStorageService, AzureBlobAvatarStorageService>();
        services.AddScoped<IAttachmentStorageService, AzureBlobAttachmentStorageService>();

        return services;
    }
}