using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class ItemCommentRepository : Repository<ItemComment, Guid>, IItemCommentRepository
{
    public ItemCommentRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    //public async Task<IReadOnlyList<BoardItemAttachment>> GetByItemAsync(Guid itemId)
    //{
    //    return await _dbSet
    //        .AsNoTracking()
    //        .Where(a => a.BoardItemId == itemId)
    //        .Include(a => a.UploadedBy)
    //        .ToListAsync();
    //}
}