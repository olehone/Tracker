using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class BoardItemAssigneeConfiguration : IEntityTypeConfiguration<BoardItemAssignee>
{
    public void Configure(EntityTypeBuilder<BoardItemAssignee> builder)
    {
        builder.ToTable("BoardItemAssignees");

        builder.HasOne(bia => bia.BoardUser)
            .WithMany(bu => bu.AssignedItems)
            .HasForeignKey(ub => ub.BoardUserId);

        builder.HasOne(bia => bia.Item)
            .WithMany(bi => bi.Assignees)
            .HasForeignKey(ub => ub.BoardItemId);

        builder.HasKey(bia => new { bia.BoardUserId, bia.BoardItemId });
    }
}