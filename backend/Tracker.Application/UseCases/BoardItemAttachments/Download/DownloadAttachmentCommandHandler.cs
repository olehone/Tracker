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
    : IRequestHandler<DownloadAttachmentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DownloadAttachmentCommand request,
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
        return await GetAttachmentUrl(request, uow, cancellationToken);

    }

    private async Task<Result> CanDownload(Board board, IUnitOfWork uow, CancellationToken cancellationToken)
    {

        if (userContext.IsUnauthenticated())
        {
            if (BoardPolicy.CanAnonView(board.Visibility))
            {
                return AuthErrors.Forbidden();
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

        var permissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, userRole);

        if (!BoardPolicy.CanView(userRole, board.Visibility, workspaceRole, boardRole))
        {
            return AuthErrors.Forbidden();
        }
        return Result.Success();
    }

    private async Task<Result<string>> GetAttachmentUrl(DownloadAttachmentCommand request, IUnitOfWork uow, CancellationToken cancellationToken)
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

        var url = await attachments.GetUrlAsync(attachment.StorageFolder,
            attachment.StorageFileName, attachment.OriginalFileName, request.ForceDirect, cancellationToken);

        if (url.IsFailure)
        {
            attachment.IsDeleted = true;
            uow.BoardItemAttachmentRepository.Update(attachment);
            await uow.SaveChangesAsync(cancellationToken);
            return Error.Gone("Attachment");
        }

        return url.Value;
    }
}
