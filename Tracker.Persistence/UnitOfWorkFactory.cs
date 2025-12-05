using Microsoft.Extensions.DependencyInjection;
using Tracker.Application.Common.UnitOfWork;

namespace Tracker.Persistence;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IUnitOfWork Create()
    {
        var db = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        return new UnitOfWork(db);
    }
}
