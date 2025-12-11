using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.Results;
using Tracker.Domain.DTOs;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;

namespace Tracker.Application.UseCases.Auth.Register;

public sealed class RegisterUserHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHasher passwordHasher) 
    : IRequestHandler<RegisterUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
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

        var sc = await uow.SaveChangesAsync();

        if (sc.IsSuccess)
        {
            return user.ToDto();
        }

        if (sc.Error.Type == ErrorType.UniqueViolation)
        {
            return AuthErrors.UsernameOrEmailExists;
        }

        return Error.Unknown;
    }
}
