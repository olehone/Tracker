using MediatR;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Tracker.Domain.Options;
using Tracker.Application.Behaviors;

namespace Tracker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddOptions<RegistrationOptions>()
            .BindConfiguration(RegistrationOptions.SectionName);
        services.AddOptions<LoginOptions>()
            .BindConfiguration(LoginOptions.SectionName);
        services.AddOptions<EntityOptions>()
            .BindConfiguration(EntityOptions.SectionName);

        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}