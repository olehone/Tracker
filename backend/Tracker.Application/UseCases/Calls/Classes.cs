namespace Tracker.Application.UseCases.Calls;


public record class TransferInfo(string SenderId, string ConnectionId);
public record class LeaveInfo(Guid UserId, bool IsCallEnded, List<string> ConnectionIds);