using Refit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;
using Tracker.Services.Abstraction;
using Tracker.Services.Auth;
using Tracker.Services.ApiClients;
using Tracker.Services.Abstraction.Auth;

namespace Tracker.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiAndServices(this IServiceCollection services)
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
            });
        services.AddApiClientWithAuth<IUserApi>();
        services.AddApiClientWithAuth<IWorkspaceApi>();
        services.AddApiClientWithAuth<IBoardsApi>();
        services.AddApiClientWithAuth<IBoardListApi>();
        services.AddApiClientWithAuth<IBoardItemApi>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkspaceService, WorkspaceService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IBoardListService, BoardListService>();
        services.AddScoped<IBoardItemService, BoardItemService>();
        return services;
    }

    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthStorage, AuthStorage>();
        services.AddScoped<IJwtTokenReader, JwtTokenReader>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        services.AddScoped<IAuthStateNotifier>(sp =>
            (CustomAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        services.AddScoped<AuthHeaderHandler>();
        services.AddAuthorizationCore();

        return services;
    }

    public static IServiceCollection AddApiClientWithAuth<TInterface>(this IServiceCollection services)
    where TInterface : class
    {
        services.AddRefitClient<TInterface>()
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(options.ApiBaseUrl);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

        return services;
    }

}
