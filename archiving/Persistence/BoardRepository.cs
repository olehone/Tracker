using ArchivingFunction.Domain.Entities;
using ArchivingFunction.Domain.Enums;
using ArchivingFunction.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace ArchivingFunction.Persistence;

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

    public void UpdateBoardArchiveStatusAsync(Guid boardId, ArchiveStatus status)
    {
        var board = new Board { Id = boardId, ArchiveStatus = status };

        dbContext.Attach(board);
        dbContext.Entry(board).Property(x => x.ArchiveStatus).IsModified = true;
    }

    public async Task DeleteBoardContentAsync(Guid boardId)
    {
        throw new NotImplementedException();
    }

    public async Task RestoreBoardContentAsync(Board snapshot)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        return dbContext.SaveChangesAsync();
    }
}