namespace Tracker.Services.Abstraction.Auth;

public interface ICurrentUser
{
    bool IsMyId(Guid checkedId);
    bool IsAuthenticated { get; }
    bool IsUnauthenticated { get; }
    Guid Id { get; }
}