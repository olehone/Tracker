using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class CommentAttachmentRepository : Repository<CommentAttachment, Guid>, ICommentAttachmentRepository
{
    public CommentAttachmentRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }
}