namespace Tracker.Domain.Options;

public class EntityOptions
{
    public const string SectionName = "EntityOptions";
    
    public required int TitleMaximumLength { get; init; }
    public required int DescriptionMaximumLength { get; init; }
}
