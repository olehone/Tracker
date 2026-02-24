using Tracker.Domain.Dtos;

namespace Tracker.Application.Common.Repositories;

public interface ICallRepository
{
    Task<CallDto?> GetCallAsync(Guid callId);
    Task SaveCallAsync(CallDto call);
    Task RemoveCallAsync(Guid callId);
    Task<UserDto?> GetUserByConnectionAsync(string connectionId);
}