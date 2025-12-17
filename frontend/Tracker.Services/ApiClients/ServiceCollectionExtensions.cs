using Refit;
using Microsoft.Extensions.Options;
using Tracker.Domain.Options;
using Microsoft.Extensions.DependencyInjection;

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
            });

        return services;
    }
}
