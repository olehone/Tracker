using Tracker.Domain.Results;

namespace Tracker.WebApp.Shared;

public interface IErrorNotifier
{
    bool NotifyIfError(Result result);
    void NotifyActionError(Error error);
    void NotifyNetworkError(Error error);
}
