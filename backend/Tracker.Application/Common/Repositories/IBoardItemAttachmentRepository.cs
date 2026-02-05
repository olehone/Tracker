using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IBoardItemAttachmentRepository : IRepository<BoardItemAttachment, Guid>
{
    Task<IReadOnlyList<BoardItemAttachment>> GetByItemAsync(Guid itemId);
}
