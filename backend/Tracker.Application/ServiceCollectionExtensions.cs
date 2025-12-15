using MediatR;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Tracker.Domain.Options;
using Tracker.Application.Behaviours;
using Tracker.Application.UseCases.Auth.Login;
using Tracker.Application.UseCases.Auth.Register;

namespace Tracker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<RegistrationOptions>()
            .BindConfiguration(RegistrationOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<LoginOptions>()
            .BindConfiguration(LoginOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}