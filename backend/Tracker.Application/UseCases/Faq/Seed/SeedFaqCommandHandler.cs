using MediatR;
using Tracker.Application.Common.Services;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Faq.Seed;

public class SeedFaqCommandHandler(IFaqService faqService) : IRequestHandler<SeedFaqCommand, Result>
{
    public Task<Result> Handle(SeedFaqCommand request, CancellationToken cancellationToken)
    {
        return faqService.SeedAsync();
    }
}