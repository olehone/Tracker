using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Update;

public class UpdateBoardCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateBoardCommand, Result>
{
    public async Task<Result> Handle(UpdateBoardCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();
        var boardResult = await BoardHelper.GetBoardForActionAsync(uow, userContext, request.BoardId, BoardAction.ChangeBoard);

        if (boardResult.IsFailure)
        {
            return boardResult.Error;
        }
        var board = boardResult.Value;
        
        var newBoard = new Board
        {
            Id = request.BoardId,
            WorkspaceId = board.WorkspaceId,
            Title = request.Title,
            Description = request.Description,
            Visibility = request.Visibility,
            PermissionRoles = request.PermissionRoles,
        };

        uow.BoardRepository.Update(newBoard);
        var result = await uow.SaveChangesAsync(cancellationToken);
        if (result.IsFailure)
        {
            return result;
        }
        return Result.Success();
    }
}