using MediatR;
using Microsoft.Extensions.Options;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Options;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Auth.Register;

public sealed class RegisterUserCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        if (await uow.UserRepository.EmailExistsAsync(request.Email))
        {
            return AuthErrors.EmailExists;
        }
        if (await uow.UserRepository.UsernameExistsAsync(request.Username))
        {
            return AuthErrors.UsernameExists;
        }

        var user = new User()
        {
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        await uow.UserRepository.AddAsync(user);

        string accessToken = tokenProvider.Create(user);

        var refreshToken = new RefreshToken()
        {
            Token = tokenProvider.GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationDays)
        };

        await uow.RefreshTokenRepository.AddAsync(refreshToken);

        var sc = await uow.SaveChangesAsync();

        if (sc.IsFailure)
        {
            return sc.Error.Type switch
            {
                ErrorType.UniqueViolation => AuthErrors.UsernameOrEmailExists,
                _ => Error.Unknown
            };
        }

        return new AuthResponse()
        {
            User = user.ToDto(),
            RefreshToken = refreshToken.ToDto(),
            AccessToken = accessToken
        };

    }
}
