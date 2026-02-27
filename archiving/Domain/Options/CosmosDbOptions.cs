namespace Domain.Options;

public class CosmosDbOptions
{
    public const string SectionName = "CosmosDbOptions";

    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
    public required string BoardArchiveLogsContainer { get; set; }
}