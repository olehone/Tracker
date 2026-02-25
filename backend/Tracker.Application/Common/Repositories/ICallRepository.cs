using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface ICallRepository
{
    Task<Call?> GetCallByIdAsync(Guid callId);
    Task<Call?> GetCallByConnectionAsync(string connectionId);
    Task SaveCallAsync(Call call);
    Task RemoveCallAsync(Guid callId);
}