namespace Domain.Options;

public class DbOptions
{
    public const string SectionName = "DbOptions";
    public required string ConnectionString { get; init; }
}