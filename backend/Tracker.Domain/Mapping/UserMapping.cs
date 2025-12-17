using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Domain.Mapping;

public static class RefreshTokenMapping
{
    public static RefreshTokenDto ToDto(this RefreshToken refreshToken)
    {
        return new RefreshTokenDto()
        {
            Token = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            IsActive = refreshToken.IsActive
        };
    }
}