using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.GetAttachments;

public class GetItemAttachmentsCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetItemAttachmentsCommand, Result<IReadOnlyList<FileDto>>>
{
    public async Task<Result<IReadOnlyList<FileDto>>> Handle(
        GetItemAttachmentsCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext,
            request.BoardItemId, request.BoardId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        var boardItem = itemResult.Value;

        var attachments = await uow.BoardItemAttachmentRepository
            .GetByItemAsync(request.BoardItemId);
        return attachments.Select(a => a.ToDto()).ToList();
    }
}