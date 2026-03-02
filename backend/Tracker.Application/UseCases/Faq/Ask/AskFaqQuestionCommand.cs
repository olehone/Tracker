using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Faq.Ask;

public class AskFaqQuestionCommand : IRequest<Result<string>>
{
    public required string Question { get; set; }
}