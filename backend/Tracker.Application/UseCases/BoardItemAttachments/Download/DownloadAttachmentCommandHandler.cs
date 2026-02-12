using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Entities;
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
        var board = await uow.BoardRepository.GetWithWorkspaceByItemAttachmentAsync(request.AttachmentId);
        if (board is null)
        {
            return Error.NotFound("Board", "attachment");
        }
        var canDownload = await CanDownload(board, uow, cancellationToken);
        if (canDownload.IsFailure)
        {
            return canDownload.Error;
        }
        return await GetAttachment(request, uow, cancellationToken);

    }

    private async Task<Result> CanDownload(Board board, IUnitOfWork uow, CancellationToken cancellationToken)
    {

        if (userContext.IsUnauthenticated())
        {
            if (BoardPolicy.CanAnonView(board.Visibility))
            {
                return AuthErrors.Forbidden("Board is private");
            }
        }

        var userId = userContext.GetUserId();
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return AuthErrors.Unauthenticated;
        }
        var userRole = user.Role;

        var workspaceRole = await uow.WorkspaceUserRepository
            .GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.BoardUserRepository
            .GetRoleAsync(userId, board.Id);

        if (!BoardPolicy.CanView(userRole, board.Visibility, workspaceRole, boardRole))
        {
            return AuthErrors.Forbidden("Board is private");
        }
        return Result.Success();
    }

    private async Task<Result<AttachmentResponse>> GetAttachment(DownloadAttachmentCommand request, 
        IUnitOfWork uow, CancellationToken cancellationToken)
    {
        var attachment = await uow.BoardItemAttachmentRepository.GetByIdAsync(request.AttachmentId);
        if (attachment is null)
        {
            return Error.NotFound("Attachment");
        }
        if (attachment.IsDeleted)
        {
            return Error.Gone("Attachment");
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
