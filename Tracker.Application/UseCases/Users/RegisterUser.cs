using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.Common.Auth;
using Tracker.Domain.Entities;
using Tracker.Domain.Exceptions;
using Tracker.Domain.DTOs;
using Tracker.Domain.Mapping;

namespace Tracker.Application.UseCases.Users;

public sealed class RegisterUser(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHasher passwordHasher)
{
    public record RegisterRequest(string Email,
        string Password,
        string Username,
        string FirstName,
        string LastName);

    public async Task<UserDto> Handle(RegisterRequest request)
    {
        await using var uow = unitOfWorkFactory.Create();

        if (await uow.UserRepository.EmailExistsAsync(request.Email))
        {
            throw new DuplicateException("Email is already in use");
        }
        if (await uow.UserRepository.UsernameExistsAsync(request.Username))
        {
            throw new DuplicateException("Username is already in use");
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

        // To avoid race condition
        try
        {
            await uow.SaveChangesAsync();
        }
        catch (DuplicateException)
        {
            throw new DuplicateException("Sorry, someone take email or username, try again");
        }

        return user.ToDto();
    }
}
