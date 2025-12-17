using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Options;

public class ApiOptions
{
    public const string SectionName = "ApiOptions";
    [Required]
    public string ApiBaseUrl { get; init; } = null!;
}
