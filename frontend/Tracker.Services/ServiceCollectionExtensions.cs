using Refit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;
using Tracker.Services.Abstraction;
using Tracker.Services.States;
using Tracker.Domain.Dtos;

namespace Tracker.Services.ApiClients;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {

        services.AddOptions<ApiOptions>()
            .BindConfiguration(ApiOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(options.ApiBaseUrl);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

        return services;
    }

    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthStorage, AuthStorage>();
        services.AddScoped<IJwtTokenReader, JwtTokenReader>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<AuthHeaderHandler>();

        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        services.AddScoped<IAuthStateNotifier, CustomAuthStateProvider>();
        services.AddScoped<AppState>();
        services.AddAuthorizationCore();

        return services;
    }
}
