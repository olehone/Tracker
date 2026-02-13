using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class ItemCommentRepository : Repository<ItemComment, Guid>, IItemCommentRepository
{
    public ItemCommentRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public new async Task<ItemComment?> GetByIdAsync(Guid commentId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.UploadedBy)
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }

    public async Task<IReadOnlyCollection<ItemComment>> LoadAsync(Guid itemId,
        DateTimeOffset before, int take)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.BoardItemId == itemId)
            .Where(c => c.UploadedAt < before)
            .OrderByDescending(c => c.UploadedAt)
            .Take(take)
            .Include(a => a.UploadedBy)
            .Include(a => a.Attachments)
            .ToListAsync();
    }
}