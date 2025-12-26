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
    private IWorkspaceRepository _workspaceRepository = null!;
    private IBoardRepository _boardRepository = null!;
    private IBoardListRepository _boardListRepository = null!;
    private IBoardItemRepository _boardItemRepository = null!;
    private IRefreshTokenRepository _refreshTokenRepository = null!;

    public IUserRepository UserRepository
        => _userRepository ??= new UserRepository(_dbContext);
    public IWorkspaceRepository WorkspaceRepository 
        => _workspaceRepository ??= new WorkspaceRepository(_dbContext);
    public IBoardRepository BoardRepository
        => _boardRepository ??= new BoardRepository(_dbContext);
    public IBoardListRepository BoardListRepository
    => _boardListRepository ??= new BoardListRepository(_dbContext);
    public IBoardItemRepository BoardItemRepository
    => _boardItemRepository ??= new BoardItemRepository(_dbContext);
    public IRefreshTokenRepository RefreshTokenRepository
        => _refreshTokenRepository ??= new RefreshTokenRepository(_dbContext);


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
            return await _dbContext.SaveChangesAsync(cancellationToken);
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
