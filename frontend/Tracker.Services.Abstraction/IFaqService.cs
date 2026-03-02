using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IFaqService
{
    Task<Result<string>> AskAsync(string question);
}