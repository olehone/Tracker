using Microsoft.Data.SqlClient;
using Tracker.Domain.Results;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Persistence.Repositories;

namespace Tracker.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    private IUserRepository _userRepository = null!;
    public IUserRepository UserRepository
        => _userRepository ??= new UserRepository(_dbContext);

    public UnitOfWork(ApplicationDbContext applicationDbContext)
    {
        _dbContext = applicationDbContext;
    }

    public ValueTask DisposeAsync()
    {
        return _dbContext.DisposeAsync();
    }

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.SaveChangesAsync();
        }
        catch (SqlException ex)
        {
            return ExceptionToError(ex);
        }
    }

    private static Error ExceptionToError(SqlException ex)
    {
        return ex switch
        {
            SqlException sqlEx when sqlEx.Number is 2601 or 2627 => PersistenceErrors.UniqueViolation,
            _ => Error.Unknown
        };
    }
}
