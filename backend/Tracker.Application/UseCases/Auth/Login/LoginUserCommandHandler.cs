using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Options;
using Microsoft.Extensions.Options;

namespace Tracker.Application.UseCases.Auth.Login;

public sealed class LoginUserCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        User? user = await uow.UserRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return AuthErrors.UserNotFound;
        }

        bool verified = passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!verified)
        {
            return AuthErrors.PasswordIsIncorrect;
        }
        string accessToken = tokenProvider.Create(user);

        var refreshToken = new RefreshToken()
        {
            Token = tokenProvider.GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationDays)
        };

        await uow.RefreshTokenRepository.AddAsync(refreshToken);
        var sc = await uow.SaveChangesAsync(cancellationToken);

        if (sc.IsSuccess)
        {
            return new AuthResponse()
            {
                User = user.ToDto(),
                AccessToken = accessToken,
                RefreshToken = refreshToken.ToDto()
            };
        }

        return Error.Unknown;
    }
}
