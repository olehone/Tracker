using Refit;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;
using Tracker.WebApp.ApiClients;

namespace Tracker.WebApp;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        services.AddOptions<ApiOptions>()
            .BindConfiguration(ApiOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddRefitClient<AuthApiClient>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(options.ApiBaseUrl);
            });

        return services;
    }
}
