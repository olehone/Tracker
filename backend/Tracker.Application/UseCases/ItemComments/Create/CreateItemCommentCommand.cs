using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.Create;

public class CreateItemCommentCommand : IRequest<Result<ItemCommentDto>>
{
    public required Guid BoardItemId { get; set; }
    public required string Content { get; set; } 
}