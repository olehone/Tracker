namespace Tracker.Services.Abstraction;

public interface IAuthStateNotifier
{
    public void NotifyUserAuthentication();
    public void NotifyUserLogout();
}
