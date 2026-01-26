using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAssignees.Remove;

public class RemoveAssigneeFromItemCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<RemoveAssigneeFromItemCommand, Result<IReadOnlySet<Guid>>>
{
    public async Task<Result<IReadOnlySet<Guid>>> Handle(RemoveAssigneeFromItemCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext,
            request.BoardItemId, request.BoardId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        var boardUser = await uow.UserBoardRepository.GetAsync(request.UserId, request.BoardId);
        if (boardUser is null)
        {
            return BoardErrors.UserNotInBoard;
        }

        var assignee = await uow.BoardItemAssigneeRepository
            .GetByUserAndItemAsync(request.UserId, request.BoardItemId);
        if (assignee is null)
        {
            return BoardErrors.UserNotAssigned;
        }
        await uow.BoardItemAssigneeRepository.RemoveAsync(assignee.Id);
        var result = await uow.SaveChangesAsync(cancellationToken);

        var item = await uow.BoardItemRepository.GetByIdAsync(request.BoardItemId);
        if (result.IsFailure || item is null)
        {
            return Error.Unknown;
        }
        return item.Assignees.Select(a => a.BoardUser.UserId).ToHashSet();
    }
}
