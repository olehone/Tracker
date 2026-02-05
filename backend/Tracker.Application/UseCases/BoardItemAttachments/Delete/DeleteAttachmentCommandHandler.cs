using MediatR;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAttachments.Delete;

public class DeleteAttachmentCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory,
    IAttachmentStorageService attachments)
    : IRequestHandler<DeleteAttachmentCommand, Result>
{
    public async Task<Result> Handle(DeleteAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext,
            request.BoardItemId, request.BoardId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }

        var attachment = await uow.BoardItemAttachmentRepository.GetByIdAsync(request.BoardItemId);
        if (attachment is null)
        {
            return Error.NotFound("Attachment");
        }
        if (attachment.IsDeleted)
        {
            return Error.Gone("Attachment");
        }

        await attachments.DeleteAsync(attachment.StorageFolder,
            attachment.StorageFileName, cancellationToken);

        attachment.IsDeleted = true;
        uow.BoardItemAttachmentRepository.Update(attachment);
        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsSuccess
            ? Result.Success()
            : Error.Unknown;
    }
}
