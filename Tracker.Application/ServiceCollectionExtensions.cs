using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using Tracker.Application.UseCases.Users;

namespace Tracker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<LoginUserHandler>();

        return services;
    }
}