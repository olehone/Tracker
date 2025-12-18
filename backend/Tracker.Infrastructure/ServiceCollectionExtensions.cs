using Microsoft.Extensions.DependencyInjection;
using Tracker.Application.Common.Auth;
using Tracker.Domain.Options;
using Tracker.Infrastructure.Auth;

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

        return services;
    }
}