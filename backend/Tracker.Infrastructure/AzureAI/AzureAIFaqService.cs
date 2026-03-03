using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Tracker.Application.Common.Services;
using Tracker.Domain.Results;

namespace Tracker.Infrastructure.AzureAI;

public partial class AzureAIFaqService(IKernelMemory kernelMemory, ILogger<AzureAIFaqService> logger)
    : IFaqService
{
    private const string FaqIndex = "faq";

    public async Task<Result> SeedAsync()
    {
        var documents = BasicFaqDocuments.GetFaqDocuments();

        foreach (var doc in documents)
        {
            await kernelMemory.ImportTextAsync(
                text: doc.Content,
                documentId: doc.Id,
                index: FaqIndex
            );
            logger.LogInformation("Imported FAQ document: {id}", doc.Id);
        }

        logger.LogInformation("FAQ seed complete. {count} documents imported.", documents.Count);
        return Result.Success();
    }

    public async Task<string> AskAsync(string question)
    {
        var result = await kernelMemory.AskAsync(
            question: question,
            index: FaqIndex,
            minRelevance: 0.5
        );

        if (result.NoResult)
        {
            return "Sorry, can't answer this question";
        }

        return result.Result;
    }

}
