using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.GetTransferInfo;

public class GetTransferInfoQuery : IRequest<Result<TransferInfo>>
{
    public required Guid CallId { get; set; }
    public required string TargetUserId { get; set; }
}
