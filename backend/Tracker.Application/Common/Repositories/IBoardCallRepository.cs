namespace Tracker.Application.Common.Repositories;

public interface IBoardCallRepository
{
    Task<Guid?> GetCallIdAsync(Guid boardId);
    Task AddCallAsync(Guid boardId, Guid callId);
    Task RemoveCallAsync(Guid boardId);
}