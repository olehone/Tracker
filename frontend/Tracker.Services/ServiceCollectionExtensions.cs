using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using Tracker.Domain.Enums;
using Tracker.Domain.Options;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Auth;
using Tracker.Services.Abstraction.Board;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.Abstraction.Workspace;
using Tracker.Services.ApiClients;
using Tracker.Services.Auth;
using Tracker.Services.Board;
using Tracker.Services.Realtime;
using Tracker.Services.Results;
using Tracker.Services.Workspace;

namespace Tracker.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiAndServices(this IServiceCollection services)
    {
        services.AddScoped<IApiErrorHandler, ApiErrorHandler>();
        services.AddOptions<ApiOptions>()
            .BindConfiguration(ApiOptions.SectionName);

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(options.ApiBaseUrl);
            });

        services.AddApiClientWithAuth<IUserApi>();
        services.AddApiClientWithAuth<IWorkspaceApi>();
        services.AddApiClientWithAuth<IWorkspaceUserApi>();
        services.AddApiClientWithAuth<IBoardsApi>();
        services.AddApiClientWithAuth<IBoardUserApi>();
        services.AddApiClientWithAuth<IBoardListApi>();
        services.AddApiClientWithAuth<IBoardItemApi>();
        services.AddApiClientWithAuth<IItemCommentApi>();
        services.AddApiClientWithAuth<IAttachmentApi>();
        services.AddApiClientWithAuth<ICallApi>();
        services.AddApiClientWithAuth<ISubscriptionApi>();
        services.AddApiClientWithAuth<IFaqApi>();

        services.AddScoped<IApiUrlService, ApiUrlService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkspaceService, WorkspaceService>();
        services.AddScoped<IWorkspaceUserService, WorkspaceUserService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IBoardUserService, BoardUserService>();
        services.AddScoped<IBoardListService, BoardListService>();
        services.AddScoped<IBoardItemService, BoardItemService>();
        services.AddScoped<IItemCommentService, ItemCommentService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<ICallService, CallService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IFaqService, FaqService>();

        services.AddScoped<IBoardRealtimeService, BoardRealtimeService>();
        services.AddScoped<IItemRealtimeService, ItemRealtimeService>();
        services.AddScoped<ICallRealtimeService, CallRealtimeService>();
        return services;
    }

    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthStorage, AuthStorage>();
        services.AddScoped<IJwtTokenReader, JwtTokenReader>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

        services.AddTransient<AuthHeaderHandler>();
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("AdminOrHigher", policy =>
                policy.RequireAssertion(ctx =>
                {
                    var roleClaim = ctx.User.FindFirst(ClaimTypes.Role) ??
                        ctx.User.FindFirst("role");

                    var role = Enum.TryParse(roleClaim?.Value, true, out GlobalRole value)
                        ? value
                        : GlobalRole.None;

                    return role >= GlobalRole.Admin;
                }));
        });

        return services;
    }

    public static IServiceCollection AddApiClientWithAuth<TInterface>(this IServiceCollection services)
    where TInterface : class
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        services.AddRefitClient<TInterface>(new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(jsonOptions)
        })
            .ConfigureHttpClient((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(options.ApiBaseUrl);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();
        //.AddStandardResilienceHandler();

        return services;
    }
}