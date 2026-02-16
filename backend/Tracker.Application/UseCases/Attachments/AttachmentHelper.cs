using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Attachments;

internal static class AttachmentHelper
{
    public static async Task<Result> DeleteAttachmentAsync(Guid attachmentId, AttachmentType type,
        IUnitOfWork uow, IUserContext userContext, IAttachmentStorageService attachments, CancellationToken cancellationToken = default, bool isIndividual = true)
    {
        Result<FileUpload> updatedAttachment = await MarkAttachmentDeletedAsync(attachmentId,
            type, uow, userContext);

        if (updatedAttachment.IsFailure)
        {
            return updatedAttachment;
        }
        var attachment = updatedAttachment.Value;

        await attachments.DeleteAsync(attachment.StorageFolder,
            attachment.StorageFileName, cancellationToken);

        if (isIndividual)
        {
            var sc = await uow.SaveChangesAsync(cancellationToken);

            return sc.IsSuccess
                ? Result.Success()
                : Error.Unknown;
        }
        return Result.Success();
    }

    public static async Task<Result<FileUpload>> GetAttachmentAsync(Guid attachmentId, AttachmentType type,
        IUnitOfWork uow, IUserContext userContext, CancellationToken cancellationToken = default)
    {
        FileUpload? attachment = type switch
        {
            AttachmentType.Item => await uow.BoardItemAttachmentRepository.GetByIdAsync(attachmentId),
            AttachmentType.Comment => await uow.CommentAttachmentRepository.GetByIdAsync(attachmentId),
            _ => throw new ArgumentException("There is no attachment type"),
        };

        if (IsAttachmentAbsent(attachment, out var error))
        {
            return error;
        }

        var board = await GetBoardByAttachmentAsync(attachmentId, type, uow);
        if (board == null)
        {
            return Error.NotFound("Board");
        }
        var canView = await BoardHelper.CanViewBoardAsync(board, uow, userContext, cancellationToken);
        if (canView.IsFailure)
        {
            return canView.Error;
        }

        return attachment!;
    }

    public static bool IsAttachmentAbsent(FileUpload? attachment, out Error error)
    {
        if (attachment is null)
        {
            error = Error.NotFound("Attachment");
            return true;
        }
        if (attachment.IsDeleted)
        {
            error = Error.Gone("Attachment");
            return true;
        }
        error = Error.None;
        return false;
    }

    public static async Task<Result<FileUpload>> MarkAttachmentDeletedAsync(Guid attachmentId, AttachmentType type,
        IUnitOfWork uow, IUserContext userContext)
    {
        return type switch
        {
            AttachmentType.Item => await MarkDeletedItemAttachmentAsync(attachmentId, uow, userContext),
            AttachmentType.Comment => await MarkDeletedCommentAttachmentAsync(attachmentId, uow, userContext),
            _ => throw new ArgumentException("There is no attachment type"),
        };
    }

    private static async Task<Board?> GetBoardByAttachmentAsync(Guid attachmentId, AttachmentType Type, IUnitOfWork uow)
    {
        return Type switch
        {
            AttachmentType.Item => await uow.BoardRepository.GetWithWorkspaceByItemAttachmentAsync(attachmentId),
            AttachmentType.Comment => await uow.BoardRepository.GetWithWorkspaceByCommentAttachmentAsync(attachmentId),
            _ => throw new ArgumentException("There is no attachment type"),
        };
    }

    private static async Task<Result<FileUpload>> MarkDeletedItemAttachmentAsync(Guid attachmentId,
        IUnitOfWork uow, IUserContext userContext)
    {
        var attachment = await uow.BoardItemAttachmentRepository.GetByIdAsync(attachmentId);
        if (IsAttachmentAbsent(attachment, out var error))
        {
            return error;
        }
        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext, attachment!.BoardItemId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        attachment!.IsDeleted = true;
        uow.BoardItemAttachmentRepository.Update(attachment);
        return attachment;
    }

    private static async Task<Result<FileUpload>> MarkDeletedCommentAttachmentAsync(Guid attachmentId,
        IUnitOfWork uow, IUserContext userContext)
    {
        var attachment = await uow.CommentAttachmentRepository.GetByIdAsync(attachmentId);
        if (IsAttachmentAbsent(attachment, out var error))
        {
            return error;
        }
        var commentResult = await BoardHelper.GetItemCommentForActionAsync(uow, userContext, attachment!.ItemCommentId);
        if (commentResult.IsFailure)
        {
            return commentResult.Error;
        }
        attachment!.IsDeleted = true;
        uow.CommentAttachmentRepository.Update(attachment);
        return attachment;
    }
}
