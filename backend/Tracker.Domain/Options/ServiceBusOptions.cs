namespace Tracker.Domain.Options;

public class ServiceBusOptions
{
    public const string SectionName = "ServiceBusOptions";

    public required string ConnectionString { get; init; }

    public required string BoardArchiveQueueName { get; init; }
    public required string BoardArchiveSubjectName { get; init; }
}