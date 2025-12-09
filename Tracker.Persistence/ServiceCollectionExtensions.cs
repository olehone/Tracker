using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tracker.Application.Common.Database;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Persistence.Repositories;
using Tracker.Persistence.Exceptions;

namespace Tracker.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddSingleton<IDbExceptionsHandler, DbExceptionsHandler>();
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}