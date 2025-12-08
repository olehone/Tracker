using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Tracker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}