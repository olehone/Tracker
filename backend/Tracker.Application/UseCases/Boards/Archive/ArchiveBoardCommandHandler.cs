using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Archive;

public class ArchiveBoardCommandHandler
    (IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext
    ) : IRequestHandler<ArchiveBoardCommand, Result>
{
    public async Task<Result> Handle(ArchiveBoardCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();
        var boardResult = await BoardHelper
            .GetBoardForActionAsync(uow, userContext, request.Id, BoardAction.ChangeArchiveStatus);

        if (boardResult.IsFailure)
        {
            return boardResult.Error;
        }

        var board = boardResult.Value;
        var updatedBoard = new Board
        {
            Id = board.Id,
            WorkspaceId = board.WorkspaceId,
            Title = board.Title,
            Description = board.Description,
            Visibility = board.Visibility,
            PermissionRoles = board.PermissionRoles,
            ArchiveStatus = ArchiveStatus.PendingArchive
        };
        uow.BoardRepository.Update(updatedBoard);

        var result = await uow.SaveChangesAsync(cancellationToken);
        return result.IsFailure ? result : Result.Success();
    }
}
