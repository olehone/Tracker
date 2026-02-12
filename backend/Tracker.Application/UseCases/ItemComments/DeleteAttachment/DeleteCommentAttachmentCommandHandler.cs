using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.BoardItemAttachments.Delete;
using Tracker.Application.UseCases.Boards;
using Tracker.Application.UseCases.ItemComments.Delete;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.DeleteAttachment;

public class DeleteCommentAttachmentCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory,
    IAttachmentStorageService attachments)
    : IRequestHandler<DeleteCommentAttachmentCommand, Result>
{
    public async Task<Result> Handle(DeleteCommentAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var attachmentResult = await BoardHelper.GetItemAttachmentForActionAsync(uow, userContext, request.AttachmentId);
        if (attachmentResult.IsFailure)
        {
            return attachmentResult.Error;
        }
        var attachment = attachmentResult.Value;

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
