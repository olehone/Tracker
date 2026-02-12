using FluentValidation;
using Microsoft.Extensions.Options;
using Tracker.Application.UseCases.ItemComments.UploadAttachment;
using Tracker.Domain.Options;

namespace Tracker.Application.UseCases.BoardItemAttachments.Upload;

public class UploadCommentAttachmentCommandValidator : AbstractValidator<UploadCommentAttachmentCommand>
{
    public UploadCommentAttachmentCommandValidator(IOptions<BlobOptions> options)
    {
        RuleFor(x => x.Content)
            .NotNull();

        RuleFor(x => x.ContentType)
            .NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty();;

        RuleFor(x => x.ContentLength)
            .GreaterThan(0)
            .WithMessage("File is empty")
            .LessThanOrEqualTo(options.Value.ItemAttachmentMaxSize)
            .WithMessage(ct => "Avatar must be less then or equal to " +
                $"{options.Value.ItemAttachmentMaxSize / (1024 * 1024)} mb.");
    }
}
