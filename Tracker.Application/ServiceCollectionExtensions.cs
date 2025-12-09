using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using Tracker.Application.UseCases.Users;

namespace Tracker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped<RegisterUser>();
        services.AddScoped<LoginUser>();

        return services;
    }
}