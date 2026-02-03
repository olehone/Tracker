using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Application.UseCases.Users.UploadAvatar;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.Users.GetAll;

public class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    public UploadAvatarCommandValidator(IOptions<BlobOptions> options)
    {
        var allowed = options.Value.AvatarContentTypes;

        RuleFor(x => x.Content)
            .NotNull();

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(ct => allowed.Contains(ct, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Only these types are allowed: {string.Join(", ", allowed)}");

        RuleFor(x => x.ContentLength)
            .GreaterThan(0)
            .WithMessage("File is empty")
            .LessThanOrEqualTo(options.Value.AvatarMaxSize)
            .WithMessage(ct => "Avatar must be less then or equal to " +
                $"{options.Value.AvatarMaxSize / (1024 * 1024)} mb.");
    }
}
