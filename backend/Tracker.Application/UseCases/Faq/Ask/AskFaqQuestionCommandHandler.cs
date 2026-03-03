using MediatR;
using Tracker.Application.Common.Services;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Faq.Ask;

public class AskFaqQuestionCommandHandler(IFaqService faqService)
    : IRequestHandler<AskFaqQuestionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AskFaqQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var result = await faqService.AskAsync(request.Question);
        return result;
    }
}