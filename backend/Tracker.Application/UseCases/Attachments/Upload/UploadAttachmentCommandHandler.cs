using MediatR;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Mapping;
using Tracker.Domain.Options;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Attachments.Upload;

public class UploadAttachmentCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory,
    IAttachmentStorageService attachments,
    IOptions<BlobOptions> options)
    : IRequestHandler<UploadAttachmentCommand, Result<FileDto>>
{
    public async Task<Result<FileDto>> Handle(UploadAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var result = await uow.SaveChangesAsync(cancellationToken);
        var attachment = request.Type switch
        {
            AttachmentType.Item => await UploadItemAttachmentAsync(uow, request, options.Value.ItemAttachmentContainerName, cancellationToken),
            AttachmentType.Comment => await UploadCommentAttachmentAsync(uow, request, options.Value.ItemAttachmentContainerName, cancellationToken),
            _ => throw new ArgumentException("There is no attachment type"),
        };
        if (attachment.IsFailure)
        {
            return attachment.Error;
        }
        return result.IsSuccess
            ? attachment.Value.ToDto(request.Type)
            : Error.Unknown;
    }

    private async Task<Result<FileUpload>> UploadItemAttachmentAsync(IUnitOfWork uow,
        UploadAttachmentCommand request, string storageFolder, CancellationToken cancellationToken)
    {
        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext, request.ParentId);
        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }

        var userId = userContext.GetUserId();

        var currentUser = await uow.UserRepository.GetByIdAsync(userId);
        if (currentUser is null)
        {
            return AuthErrors.Unauthenticated;
        }
        var storageFileName = await attachments.UploadAsync(request.Content,
            storageFolder, request.ContentType, cancellationToken);


        var attachment = new BoardItemAttachment
        {
            BoardItemId = request.ParentId,
            UserId = userId,
            UploadedBy = currentUser,
            OriginalFileName = request.FileName,
            ContentType = request.ContentType,
            SizeBytes = request.ContentLength,
            StorageFileName = storageFileName,
            StorageFolder = storageFolder,
        };

        await uow.BoardItemAttachmentRepository.AddAsync(attachment);
        return attachment;
    }

    private async Task<Result<FileUpload>> UploadCommentAttachmentAsync(IUnitOfWork uow,
        UploadAttachmentCommand request, string storageFolder, CancellationToken cancellationToken)
    {
        var commentResult = await BoardHelper.GetItemCommentForActionAsync(uow, userContext, request.ParentId);
        if (commentResult.IsFailure)
        {
            return commentResult.Error;
        }

        var userId = userContext.GetUserId();

        var currentUser = await uow.UserRepository.GetByIdAsync(userId);
        if (currentUser is null)
        {
            return AuthErrors.Unauthenticated;
        }
        var storageFileName = await attachments.UploadAsync(request.Content,
            storageFolder, request.ContentType, cancellationToken);

        var attachment = new CommentAttachment
        {
            ItemCommentId = request.ParentId,
            UserId = userId,
            UploadedBy = currentUser,
            OriginalFileName = request.FileName,
            ContentType = request.ContentType,
            SizeBytes = request.ContentLength,
            StorageFileName = storageFileName,
            StorageFolder = storageFolder,
        };

        await uow.CommentAttachmentRepository.AddAsync(attachment);
        return attachment;
    }
}