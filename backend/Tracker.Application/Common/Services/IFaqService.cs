using Tracker.Domain.Results;

namespace Tracker.Application.Common.Services;

public interface IFaqService
{
    Task<Result> SeedAsync();
    Task<string> AskAsync(string question);
}
