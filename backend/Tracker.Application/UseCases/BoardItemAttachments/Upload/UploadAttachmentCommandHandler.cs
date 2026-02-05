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

namespace Tracker.Application.UseCases.BoardItemAttachments.Upload;

public class UploadAttachmentCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory,
    IAttachmentStorageService attachments,
    IOptions<BlobOptions> options)
    : IRequestHandler<UploadAttachmentCommand, Result<BoardItemAttachmentDto>>
{
    public async Task<Result<BoardItemAttachmentDto>> Handle(UploadAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var itemResult = await BoardHelper.GetBoardItemForActionAsync(uow, userContext,
            request.BoardItemId, request.BoardId);
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
        var storageFolder = options.Value.ItemAttachmentContainerName;
        var storageFileName = await attachments.UploadAsync(request.Content,
            storageFolder, request.ContentType, cancellationToken);


        var newAttachment = new BoardItemAttachment
        {
            BoardItemId = request.BoardItemId,
            UserId = userId,
            OriginalFileName = request.FileName,
            ContentType = request.ContentType,
            SizeBytes = request.ContentLength,
            StorageFileName = storageFileName,
            StorageFolder = storageFolder,
        };

        await uow.BoardItemAttachmentRepository.AddAsync(newAttachment);
        var result = await uow.SaveChangesAsync(cancellationToken);

        newAttachment.UploadedBy = currentUser;
        return result.IsSuccess
            ? newAttachment.ToDto()
            : Error.Unknown;
    }
}