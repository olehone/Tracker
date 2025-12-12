using Microsoft.Extensions.DependencyInjection;
using Tracker.Application.Common.Auth;
using Tracker.Infrastructure.Auth;

namespace Tracker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenProvider, TokenProvider>();

        return services;
    }
}