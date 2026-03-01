namespace Tracker.Domain.Options;

public class AIOptions
{
    public const string SectionName = "AIOptions";

    public required string OpenAIEndpoint { get; init; }
    public required string OpenAIApiKey { get; init; }
    public required string AzureAISearchEndpoint { get; init; }
    public required string AzureAISearchApiKey { get; init; }
    public required string Deployment { get; init; }
    public required string EmbeddingDeployment { get; init; }
}