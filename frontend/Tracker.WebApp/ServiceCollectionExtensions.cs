using Blazored.LocalStorage;
using MudBlazor;
using MudBlazor.Services;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMudBlazorServices(this IServiceCollection services)
    {
        services.AddScoped<AppState>();
        services.AddScoped<IErrorNotifier, GlobalSnackbarMessages>();
        services.AddScoped<IResultNotifier, GlobalSnackbarMessages>();
        services.AddBlazoredLocalStorage();
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
}