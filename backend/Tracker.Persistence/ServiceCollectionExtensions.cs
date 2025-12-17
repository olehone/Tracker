using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Options;
using Tracker.Persistence.Repositories;

namespace Tracker.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services)
    {
        services.AddOptions<DbConnectionOptions>()
            .BindConfiguration(DbConnectionOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
        {
            var dbOptions = serviceProvider.GetRequiredService<IOptions<DbConnectionOptions>>().Value;
            optionsBuilder.UseSqlServer(dbOptions.DefaultConnectionString);
        });
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}