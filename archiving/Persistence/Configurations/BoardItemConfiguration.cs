using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ArchivingFunction.Domain.Entities;

namespace ArchivingFunction.Persistence.Configurations;

public class BoardItemConfiguration : IEntityTypeConfiguration<BoardItem>
{
    public void Configure(EntityTypeBuilder<BoardItem> builder)
    {
        builder.ToTable("BoardItems");
        builder.HasKey(bi => bi.Id);
        builder.Property(bi => bi.Title).IsRequired();
        builder.Property(bi => bi.IsDone).IsRequired();
        builder.Property(bi => bi.DueDate).HasPrecision(0);

        builder.HasMany(bi => bi.Assignees)
            .WithOne(a => a.Item)
            .HasForeignKey(a => a.BoardItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(bi => bi.Comments)
            .WithOne()
            .HasForeignKey(c => c.BoardItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(bi => bi.Attachments)
            .WithOne(a => a.Item)
            .HasForeignKey(a => a.BoardItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}