namespace Tracker.Application.UseCases.Calls;


public record class TransferInfo(string SenderId, string ConnectionId);
public record class DisconnectInfo(Guid CallId, LeaveInfo LeaveInfo);
public record class LeaveInfo(Guid UserId, bool CallEnded, List<string> ConnectionIds);