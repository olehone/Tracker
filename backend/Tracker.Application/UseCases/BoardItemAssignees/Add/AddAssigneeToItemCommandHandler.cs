using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAssignees.Add;

public class AddAssigneeToItemCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<AddAssigneeToItemCommand, Result<IReadOnlySet<Guid>>>
{
    public async Task<Result<IReadOnlySet<Guid>>> Handle(AddAssigneeToItemCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext,
            request.BoardItemId, request.BoardId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        var boardUser = await uow.BoardUserRepository.GetAsync(request.UserId, request.BoardId);
        if (boardUser is null)
        {
            return BoardErrors.UserNotInBoard;
        }
        var assignee = await uow.BoardItemAssigneeRepository
            .GetByUserAndItemAsync(request.UserId, request.BoardItemId);
        if (assignee is not null)
        {
            return BoardErrors.UserAlreadyAssigned;
        }

        var newAssignee = new BoardItemAssignee
        {
            BoardItemId = request.BoardItemId,
            BoardUserId = boardUser.Id,
        };

        await uow.BoardItemAssigneeRepository.AddAsync(newAssignee);
        var result = await uow.SaveChangesAsync(cancellationToken);

        var item = await uow.BoardItemRepository.GetByIdAsync(request.BoardItemId);
        if (result.IsFailure || item is null)
        {
            return Error.Unknown;
        }
        return item.Assignees.Select(a => a.BoardUser.UserId).ToHashSet();
    }
}
