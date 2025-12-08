using Tracker.Application.Common;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;

namespace Tracker.Application.UseCases.Users;

public sealed class RegisterUser(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHasher passwordHasher)
{
    public record Request(string Email, 
        string Password, 
        string Username, 
        string FirstName, 
        string LastName);

    public async Task<User> Handle(Request request)
    {
        await using var uow =  unitOfWorkFactory.Create();

        if (await uow.Users.EmailExistsAsync(request.Email))
        {
            throw new Exception("Email is already in use");
        }
        if (await uow.Users.UsernameExistsAsync(request.Username))
        {
            throw new Exception("Username is already in use");
        }

        var user = new User()
        {
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        await uow.Users.AddAsync(user);

        return user;
    }
}
