using Tracker.Application.Common.Repositories;
using Tracker.Domain.Results;

namespace Tracker.Application.Common.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    IUserSubscriptionRepository UserSubscriptionRepository { get; }
    IWorkspaceRepository WorkspaceRepository { get; }
    IWorkspaceUserRepository WorkspaceUserRepository { get; }
    IBoardRepository BoardRepository { get; }
    IBoardUserRepository BoardUserRepository { get; }
    IBoardListRepository BoardListRepository { get; }
    IBoardItemRepository BoardItemRepository { get; }
    IItemCommentRepository ItemCommentRepository { get; }
    IBoardItemAssigneeRepository BoardItemAssigneeRepository { get; }
    IBoardItemAttachmentRepository BoardItemAttachmentRepository { get; }
    ICommentAttachmentRepository CommentAttachmentRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    IRoadmapRepository RoadmapRepository { get; }

    Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default);
}
