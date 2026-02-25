using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface ICallService
{
    Task<Result<CallDto>> GetByIdAsync(Guid id);
}