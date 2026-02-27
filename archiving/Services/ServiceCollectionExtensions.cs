using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;

namespace Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddScoped<IBoardArchivingService, BoardArchivingService>();
        return services;
    }
}