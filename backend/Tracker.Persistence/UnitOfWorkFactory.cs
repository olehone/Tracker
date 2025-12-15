using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.UnitOfWork;

namespace Tracker.Persistence;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    
    public UnitOfWorkFactory(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public IUnitOfWork Create()
    {
        var context = _dbContextFactory.CreateDbContext();
        return new UnitOfWork(context);
    }
}
