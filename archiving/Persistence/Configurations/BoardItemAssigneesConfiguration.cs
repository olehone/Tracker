using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArchivingFunction.Domain.Entities;

namespace ArchivingFunction.Persistence.Configurations;

public class BoardItemAssigneeConfiguration : IEntityTypeConfiguration<BoardItemAssignee>
{
    public void Configure(EntityTypeBuilder<BoardItemAssignee> builder)
    {
        builder.ToTable("BoardItemAssignees");

        builder.HasOne(bia => bia.Item)
            .WithMany(bi => bi.Assignees)
            .HasForeignKey(ub => ub.BoardItemId);

        builder.HasKey(bia => new { bia.BoardUserId, bia.BoardItemId });
    }
}