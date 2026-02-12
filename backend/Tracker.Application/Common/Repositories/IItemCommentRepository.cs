using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IItemCommentRepository : IRepository<ItemComment, Guid>
{
    Task<IReadOnlyCollection<ItemComment>> LoadAsync(Guid itemId, DateTimeOffset before, int take);
}
