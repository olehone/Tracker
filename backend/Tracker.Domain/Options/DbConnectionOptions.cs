using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Options;

public class DbConnectionOptions
{
    public const string SectionName = "DbOptions";
    [Required]
    public required string DefaultConnectionString { get; init; }
}