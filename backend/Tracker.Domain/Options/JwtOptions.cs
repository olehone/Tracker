using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Options;

public class JwtOptions
{
    public const string SectionName = "JwtOptions";
    [Required]
    public required string Issuer { get; init; }
    [Required]
    public required string Audience { get; init; }
    [Required]
    public required string SecretKey { get; init; }
    [Required]
    public int ExpirationInMinutes { get; init; }
    [Required]
    public int RefreshTokenExpirationDays { get; init; }
    [Required]
    public int RefreshTokenBytes { get; init; }
}
