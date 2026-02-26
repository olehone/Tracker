namespace Tracker.Application.Common.States;

public interface IBoardCallState
{
    Task<Guid?> GetCallIdAsync(Guid boardId);
    Task AddCallAsync(Guid boardId, Guid callId);
    Task RemoveCallAsync(Guid boardId);
}