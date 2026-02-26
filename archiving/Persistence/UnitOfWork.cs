using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Persistence.Repositories;

namespace Tracker.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    private IBoardRepository _boardRepository = null!;
    private IBoardListRepository _boardListRepository = null!;
    private IBoardItemRepository _boardItemRepository = null!;
    private IItemCommentRepository _itemCommentRepository = null!;
    private IBoardItemAssigneeRepository _boardItemAssigneeRepository = null!;
    private IBoardItemAttachmentRepository _boardItemAttachmentRepository = null!;
    private ICommentAttachmentRepository _commentAttachmentRepository = null!;

    public IBoardRepository BoardRepository
        => _boardRepository ??= new BoardRepository(_dbContext);
    public IBoardListRepository BoardListRepository
        => _boardListRepository ??= new BoardListRepository(_dbContext);
    public IBoardItemRepository BoardItemRepository
        => _boardItemRepository ??= new BoardItemRepository(_dbContext);
    public IItemCommentRepository ItemCommentRepository
        => _itemCommentRepository ??= new ItemCommentRepository(_dbContext);
    public IBoardItemAssigneeRepository BoardItemAssigneeRepository
        => _boardItemAssigneeRepository ??= new BoardItemAssigneeRepository(_dbContext);
    public IBoardItemAttachmentRepository BoardItemAttachmentRepository
        => _boardItemAttachmentRepository ??= new BoardItemAttachmentRepository(_dbContext);
    public ICommentAttachmentRepository CommentAttachmentRepository
        => _commentAttachmentRepository ??= new CommentAttachmentRepository(_dbContext);

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
        catch (Exception ex)
        {
            return ExceptionToError(ex);
        }
    }

    private static Error ExceptionToError(Exception ex)
    {
        return ex switch
        {
            SqlException sqlEx when sqlEx.Number is 2601 or 2627 => PersistenceErrors.UniqueViolation,
            _ => Error.Unknown
        };
    }
}
