using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.GetUserConnection;

public class GetTransferInfoQuery : IRequest<Result<TransferInfo>>
{
    public required Guid CallId { get; set; }
    public required string TargetUserId { get; set; }
}
