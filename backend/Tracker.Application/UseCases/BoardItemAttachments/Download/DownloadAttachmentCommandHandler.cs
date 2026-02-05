using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAttachments.Download;

public class DownloadAttachmentCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory,
    IAttachmentStorageService attachments)
    : IRequestHandler<DownloadAttachmentCommand, Result<AttachmentResponse>>
{
    public async Task<Result<AttachmentResponse>> Handle(DownloadAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext,
            request.BoardItemId, request.BoardId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }

        var attachment = await uow.BoardItemAttachmentRepository.GetByIdAsync(request.AttachmentId);
        if (attachment is null)
        {
            return Error.NotFound("Attachment");
        }
        if (attachment.IsDeleted)
        {
            return Error.Gone("Attachment");
        }
        if (attachment.BoardItemId != request.BoardItemId)
        {
            return Error.Validation("Item does not have this attachment");
        }

        var response = new AttachmentResponse
        {
            ContentType = attachment.ContentType,
            FileName = attachment.OriginalFileName,
        };
        bool isFailure;

        if (request.ForceDirect)
        {
            var stream = await attachments.GetStreamAsync(attachment.StorageFolder,
                attachment.StorageFileName, cancellationToken);
            isFailure = stream.IsFailure;
            response.Stream = stream.Value;
        }
        else
        {
            var url = await attachments.GetUrlAsync(attachment.StorageFolder,
                attachment.StorageFileName, attachment.OriginalFileName, request.ForceDirect, cancellationToken);
            isFailure = url.IsFailure;
            response.RedirectUrl = url.Value;
        }

        if (isFailure)
        {
            attachment.IsDeleted = true;
            uow.BoardItemAttachmentRepository.Update(attachment);
            await uow.SaveChangesAsync(cancellationToken);
            return Error.Gone("Attachment");
        }

        return response;
    }
}
