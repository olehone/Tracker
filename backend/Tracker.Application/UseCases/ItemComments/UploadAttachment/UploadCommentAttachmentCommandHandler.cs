using MediatR;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Options;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.UploadAttachment;

public class UploadCommentAttachmentCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory,
    IAttachmentStorageService attachments,
    IOptions<BlobOptions> options)
    : IRequestHandler<UploadCommentAttachmentCommand, Result<CommentAttachmentDto>>
{
    public async Task<Result<CommentAttachmentDto>> Handle(UploadCommentAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var commentResult = await BoardHelper.GetItemCommentForActionAsync(uow, userContext,
            request.CommentId);
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
        var storageFolder = options.Value.ItemAttachmentContainerName;
        var storageFileName = await attachments.UploadAsync(request.Content,
            storageFolder, request.ContentType, cancellationToken);

        var newAttachment = new CommentAttachment
        {
            ItemCommentId = request.CommentId,
            UserId = userId,
            OriginalFileName = request.FileName,
            ContentType = request.ContentType,
            SizeBytes = request.ContentLength,
            StorageFileName = storageFileName,
            StorageFolder = storageFolder,
        };

        await uow.CommentAttachmentRepository.AddAsync(newAttachment);
        var result = await uow.SaveChangesAsync(cancellationToken);

        newAttachment.UploadedBy = currentUser;
        return result.IsSuccess
            ? newAttachment.ToDto()
            : Error.Unknown;
    }
}