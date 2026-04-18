using System.Text.RegularExpressions;
using Tracker.Application.Common.Services;
using Tracker.Domain.Results;

namespace Tracker.Infrastructure.AzureAI;

public partial class FaqServiceMock : IFaqService
{
    private const string FaqIndex = "faq";

    public async Task<Result> SeedAsync()
    {
        return Result.Success();
    }

    public async Task<string> AskAsync(string question)
    {
        Task.Delay(2000).Wait();
        var clean = Regex.Replace(question.ToLower(), @"[^\w\s]", "");

        var words = clean.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            if (BasicFaqKeywords.Map.TryGetValue(word, out var faqId))
            {
                var faq = BasicFaqDocuments
                    .GetFaqDocuments()
                    .FirstOrDefault(x => x.Id == faqId);

                if (faq != null)
                {
                    return faq.Answer;
                }
            }
        }

        return "I dont understand the question";
    }


}

public static class BasicFaqKeywords
{
    public static readonly Dictionary<string, string> Map = new()
    {
        // subscription
        ["subscription"] = "faq-sub-001",
        ["plan"] = "faq-sub-001",
        ["pricing"] = "faq-sub-001",

        ["upgrade"] = "faq-sub-002",
        ["cancel"] = "faq-sub-002",
        ["unsubscribe"] = "faq-sub-002",
        ["billing"] = "faq-sub-002",
        ["payment"] = "faq-sub-002",

        ["downgrade"] = "faq-sub-003",
        ["free"] = "faq-sub-003",

        // workspace
        ["workspace"] = "faq-workspace-001",
        ["team"] = "faq-workspace-001",

        ["invite"] = "faq-workspace-002",
        ["member"] = "faq-workspace-002",
        ["role"] = "faq-workspace-002",

        // board
        ["board"] = "faq-board-001",
        ["create"] = "faq-board-001",

        // items
        ["item"] = "faq-items-001",
        ["task"] = "faq-items-001",
        ["manage"] = "faq-items-001",

        ["assign"] = "faq-items-002",
        ["assignee"] = "faq-items-002",

        // attachments
        ["file"] = "faq-attach-001",
        ["attachment"] = "faq-attach-001",
        ["upload"] = "faq-attach-001",

        // comments
        ["comment"] = "faq-comments-001",
        ["message"] = "faq-comments-001",

        // calls
        ["call"] = "faq-calls-001",
        ["video"] = "faq-calls-001",
        ["meeting"] = "faq-calls-001",

        // calendar
        ["calendar"] = "faq-calendar-001",
        ["date"] = "faq-calendar-001",

        // eisenhower
        ["eisenhower"] = "faq-eisenhower-001",
        ["priority"] = "faq-eisenhower-001",

        // archive
        ["archive"] = "faq-archive-001"
    };
}