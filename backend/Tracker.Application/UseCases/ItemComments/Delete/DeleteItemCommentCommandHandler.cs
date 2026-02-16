using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Attachments;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.Delete;

public class DeleteItemCommentCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory,
    IAttachmentStorageService attachments)
    : IRequestHandler<DeleteItemCommentCommand, Result>
{
    public async Task<Result> Handle(
        DeleteItemCommentCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var commentResult = await BoardHelper.GetItemCommentForActionAsync(uow, userContext,
            request.CommentId);
        if (commentResult.IsFailure)
        {
            return commentResult.Error;
        }
        var comment = commentResult.Value;

        foreach (var attachment in comment.Attachments)
        {
            await AttachmentHelper.DeleteAttachmentAsync(attachment.Id, AttachmentType.Comment,
                uow, userContext, attachments, cancellationToken, isIndividual: false);
        }

        await uow.ItemCommentRepository.RemoveAsync(comment.Id);

        var sc = await uow.SaveChangesAsync(cancellationToken);
        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }
}