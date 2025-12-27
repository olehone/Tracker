namespace Tracker.Services.Abstraction.Auth;

public interface IAuthStateNotifier
{
    public void NotifyUserAuthentication();
    public void NotifyUserLogout();
}
