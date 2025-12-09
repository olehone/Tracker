using Microsoft.Extensions.DependencyInjection;
using Tracker.Application.Common.Auth;
using Tracker.Infrastructure.Auth;

namespace Tracker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        return services;
    }
}