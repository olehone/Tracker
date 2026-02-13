using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Attachments.Download;

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

        var attachment = await AttachmentHelper.GetAttachmentAsync(request.AttachmentId, request.Type,
            uow, userContext, cancellationToken);
        if (attachment.IsFailure)
        {
            return attachment.Error;
        }

        var attachmentResponse = await GetAttachmentResponseAsync(attachment.Value, request.ForceDirect, cancellationToken);
        if (attachmentResponse.IsFailure)
        {
            var remove = await AttachmentHelper.MarkAttachmentDeletedAsync(request.AttachmentId, request.Type,
                uow, userContext);
            return remove.IsFailure
                ? remove.Error
                : attachmentResponse.Error;
        }
        return attachmentResponse;
    }

    private async Task<Result<AttachmentResponse>> GetAttachmentResponseAsync(FileUpload attachment, bool forceDirect,
        CancellationToken cancellationToken)
    {
        var response = new AttachmentResponse
        {
            ContentType = attachment!.ContentType,
            FileName = attachment!.OriginalFileName,
        };

        if (forceDirect)
        {
            var stream = await attachments.GetStreamAsync(attachment.StorageFolder,
                attachment.StorageFileName, cancellationToken);
            if (stream.IsFailure)
            {
                return stream.Error;
            }
            response.Stream = stream.Value;
        }
        else
        {
            var url = await attachments.GetUrlAsync(attachment.StorageFolder,
                attachment.StorageFileName, attachment.OriginalFileName, forceDirect, cancellationToken);
            if (url.IsFailure)
            {
                return url.Error;
            }
            response.RedirectUrl = url.Value;
        }
        return response;
    }
}
