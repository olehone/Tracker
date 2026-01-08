using Tracker.Domain.Results;

namespace Tracker.WebApp.Shared;

public interface IResultNotifier
{
    void Notify(Result result, string? successMessage = null);
}