using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class BoardItemAssigneeConfiguration : BaseEntityConfiguration<BoardItemAssignee>
{
    public override void Configure(EntityTypeBuilder<BoardItemAssignee> builder)
    {
        base.Configure(builder);

        builder.ToTable("BoardItemAssignees");

        builder.HasIndex(a => new { a.BoardUserId, a.BoardItemId }).IsUnique();
    }
}
