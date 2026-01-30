using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardUsers.Change;

public class ChangeBoardUserRoleCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<ChangeBoardUserRoleCommand, Result>
{
    public async Task<Result> Handle(
        ChangeBoardUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var action = request.Role == BoardUserRole.Owner
            ? BoardAction.ChangeOwner
            : BoardAction.ChangeBoard;

        var boardResult = await BoardHelper.GetBoardForActionAsync(uow, userContext,
            request.BoardId, action);
        if (boardResult.IsFailure)
        {
            return boardResult.Error;
        }

        var user = await uow.UserRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        var boardUser = await uow.BoardUserRepository.GetAsync(request.UserId, request.BoardId);
        if (boardUser is null)
        {
            return BoardErrors.UserNotInBoard;
        }

        boardUser.Role = request.Role;

        if (action == BoardAction.ChangeOwner)
        {
            var oldOwner = await uow.BoardUserRepository.GetOwnerAsync(boardUser.BoardId);
            oldOwner!.Role = BoardUserRole.Admin;

            uow.BoardUserRepository.Update(oldOwner);

        }
        uow.BoardUserRepository.Update(boardUser);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }
}