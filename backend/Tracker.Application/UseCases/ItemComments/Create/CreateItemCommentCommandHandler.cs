using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.Create;

public class CreateItemCommentCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<CreateItemCommentCommand, Result<ItemCommentDto>>
{
    public async Task<Result<ItemCommentDto>> Handle(
        CreateItemCommentCommand request, 
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext,
            request.BoardItemId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        var item = itemResult.Value;
        var userId = userContext.GetUserId();

        var itemComment = new ItemComment
        {
            UserId = userId,
            BoardItemId = request.BoardItemId,
            Content = request.Content,
        };
        await uow.ItemCommentRepository.AddAsync(itemComment);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : itemComment.ToDto();
    }
}