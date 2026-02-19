using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Results;

public interface IResultNotifier
{
    void Notify(Result result, string? successMessage = null);
}