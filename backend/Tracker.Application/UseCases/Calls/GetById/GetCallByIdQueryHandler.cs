using MediatR;
using Tracker.Application.Common.Repositories;
using Tracker.Application.UseCases.Calls.Get;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Calls.GetById;

public class GetCallByIdQueryHandler(ICallRepository repo)
    : IRequestHandler<GetCallByIdQuery, Result<CallDto>>
{
    public async Task<Result<CallDto>> Handle(GetCallByIdQuery request, CancellationToken cancellationToken)
    {
        var call = await repo.GetCallByIdAsync(request.Id);
        if (call is null)
        {
            return Error.NotFound("Call");
        }

        return call.ToDto();
    }
}
