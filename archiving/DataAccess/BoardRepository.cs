using DataAccess.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class BoardRepository(ApplicationDbContext dbContext)
    : IBoardRepository
{
    public async Task<Board?> LoadFullBoardAsync(Guid boardId)
    {
        return await dbContext.Boards
            .AsNoTracking()
            .AsSplitQuery()
            .Include(b => b.BoardLists)
                .ThenInclude(l => l.BoardItems)
                    .ThenInclude(i => i.Assignees)
            .Include(b => b.BoardLists)
                .ThenInclude(l => l.BoardItems)
                    .ThenInclude(i => i.Attachments)
            .Include(b => b.BoardLists)
                .ThenInclude(l => l.BoardItems)
                    .ThenInclude(i => i.Comments)
                        .ThenInclude(c => c.Attachments)
            .FirstOrDefaultAsync(b => b.Id == boardId);
    }

    public async Task UpdateBoardArchiveStatusAsync(Guid boardId, ArchiveStatus status)
    {
        await dbContext.Boards
            .Where(b => b.Id == boardId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(b => b.ArchiveStatus, status));
    }

    public async Task DeleteBoardContentAsync(Guid boardId)
    {
        var lists = await dbContext.BoardLists
            .AsNoTracking()
            .Where(bl => bl.BoardId == boardId)
            .ExecuteDeleteAsync();
    }

    public void RestoreBoardContent(Board snapshot)
    {
        dbContext.BoardLists.AddRange(snapshot.BoardLists);
    }

    public Task SaveChangesAsync()
    {
        return dbContext.SaveChangesAsync();
    }
}