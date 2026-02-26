using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class BoardItemConfiguration : BaseEntityConfiguration<BoardItem>
{
    public override void Configure(EntityTypeBuilder<BoardItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("BoardItems");

        builder.Property(bi => bi.Title).IsRequired();
        builder.Property(bi => bi.IsDone).IsRequired();
        builder.Property(bi => bi.Position).IsRequired();
        builder.Property(bi => bi.DueDate).HasPrecision(0);

        builder.HasMany(bi => bi.Assignees)
            .WithOne()
            .HasForeignKey(a => a.BoardItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(bi => bi.Attachments)
            .WithOne()
            .HasForeignKey(a => a.BoardItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(bi => bi.Comments)
            .WithOne()
            .HasForeignKey(c => c.BoardItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
