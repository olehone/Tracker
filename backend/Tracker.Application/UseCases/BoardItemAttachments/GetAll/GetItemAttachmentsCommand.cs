using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAttachments.GetAll;

public class GetItemAttachmentsCommand : IRequest<Result<IReadOnlyList<FileDto>>>
{
    public required Guid BoardId { get; set; }
    public required Guid BoardItemId { get; set; }
}