using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Results;

public interface IErrorNotifier
{
    bool NotifyIfError(Result result);
    void NotifyActionError(Error error);
    void NotifyNetworkError(Error error);
}