using MediatR;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Options;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.Refresh;

public sealed class RefreshUserTokenCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    ITokenProvider tokenProvider,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<RefreshUserTokenCommand, Result<TokensDto>>
{
    public async Task<Result<TokensDto>> Handle(
        RefreshUserTokenCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        RefreshToken? refreshToken = await uow.RefreshTokenRepository
            .GetByTokenAsync(request.RefreshToken);

        if (refreshToken == null || refreshToken.User == null)
        {
            return AuthErrors.RefreshTokenNotFound;
        }
        if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return AuthErrors.RefreshTokenExpired;
        }

        refreshToken.Token = tokenProvider.GenerateRefreshToken();
        refreshToken.ExpiresAt = DateTimeOffset.UtcNow
            .AddDays(jwtOptions.Value.RefreshTokenExpirationDays);
        
        await uow.SaveChangesAsync(cancellationToken);

        return new TokensDto()
        {
            AccessToken = tokenProvider.Create(refreshToken.User),
            RefreshToken = refreshToken.Token
        };
    }
}
