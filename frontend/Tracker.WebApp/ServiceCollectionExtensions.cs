using Blazored.LocalStorage;
using MudBlazor;
using MudBlazor.Services;
using Tracker.Services.Abstraction.Results;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;
using Soenneker.Blazor.Drawflow.Registrars;

namespace Tracker.WebApp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMudBlazorServices(this IServiceCollection services)
    {
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
        });

        return services;
    }

    public static IServiceCollection AddStates(this IServiceCollection services)
    {
        services.AddScoped<AppState>();
        services.AddScoped<CallState>();
        services.AddScoped<BoardState>();
        return services;
    }

    public static IServiceCollection AddOtherServices(this IServiceCollection services)
    {
        services.AddScoped<IErrorNotifier, GlobalSnackbarMessages>();
        services.AddScoped<IResultNotifier, GlobalSnackbarMessages>();
        services.AddBlazoredLocalStorage();
        return services;
    }

    public static IServiceCollection AddDrawflow(this IServiceCollection services)
    {
        services.AddDrawflowInteropAsScoped();
        return services;
    }
}