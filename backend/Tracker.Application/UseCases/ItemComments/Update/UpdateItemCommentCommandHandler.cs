using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.Update;

public class UpdateItemCommentCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateItemCommentCommand, Result<ItemCommentDto>>
{
    public async Task<Result<ItemCommentDto>> Handle(UpdateItemCommentCommand request,
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

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;
        
        uow.ItemCommentRepository.Update(comment);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : comment.ToDto();
    }
}