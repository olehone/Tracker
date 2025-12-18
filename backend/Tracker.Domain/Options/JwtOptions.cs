using System.ComponentModel.DataAnnotations;

namespace Tracker.Domain.Options;

public class JwtOptions
{
    public const string SectionName = "JwtOptions";
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SecretKey { get; init; }
    public int ExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationDays { get; init; }
    public int RefreshTokenBytes { get; init; }
}
