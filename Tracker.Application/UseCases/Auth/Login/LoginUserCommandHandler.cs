using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;
using Tracker.Domain.Entities;

namespace Tracker.Application.UseCases.Auth.Login;

public sealed class LoginUserCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) 
    : IRequestHandler<LoginUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
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
        string token = tokenProvider.Create(user);

        return token;
    }
}
