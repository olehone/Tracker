using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.Common.Auth;
using Tracker.Domain.Entities;
using MediatR;

namespace Tracker.Application.UseCases.Users;

public sealed class LoginUserHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : IRequestHandler<LoginUserCommand, string>
{
    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        User? user = await uow.UserRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            throw new Exception("User is not found");
        }

        bool verified = passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!verified)
        {
            throw new Exception("Password is incorrect");
        }
        string token = tokenProvider.Create(user);

        return token;
    }
}
