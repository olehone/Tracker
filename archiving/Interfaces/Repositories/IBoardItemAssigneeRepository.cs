using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IBoardItemAssigneeRepository : IRepository<BoardItemAssignee, Guid>
{
    Task<BoardItemAssignee?> GetByUserAndItemAsync(Guid userId, Guid itemId);
}
