using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.Get;

public class LoadCommentsQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext)
    : IRequestHandler<LoadCommentsQuery, Result<CursorPage<ItemCommentDto>>>
{
    public async Task<Result<CursorPage<ItemCommentDto>>> Handle(
        LoadCommentsQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetItemAsync(uow, userContext, request.ItemId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        var takePlusOne = request.Take + 1;
        var entities = await uow.ItemCommentRepository
            .LoadAsync(itemResult.Value.Id, request.Before, takePlusOne);

        var hasMore = entities.Count > request.Take;

        var comments = entities
            .Take(request.Take)
            .Select(c => c.ToDto())
            .ToList();
        var last = comments.LastOrDefault();

        return new CursorPage<ItemCommentDto>
        {
            Items = comments,
            LastLoadedAt = hasMore ? last!.UploadedAt : null,
            HasMore = hasMore
        };
    }
}