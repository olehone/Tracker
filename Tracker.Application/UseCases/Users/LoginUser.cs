using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.Common.Auth;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.DTOs;

namespace Tracker.Application.UseCases.Users;

public sealed class LoginUser(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider)
{
    public record LoginRequest(string Email, string Password);

    public async Task<string> Handle(LoginRequest request)
    {
        await using var uow =  unitOfWorkFactory.Create();

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
