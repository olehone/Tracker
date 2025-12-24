using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class BoardListConfiguration : IEntityTypeConfiguration<BoardList>
{
    public void Configure(EntityTypeBuilder<BoardList> builder)
    {
        builder.ToTable("BoardLists");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired();

        builder.HasMany(bl => bl.BoardItems)
            .WithOne(bi => bi.BoardList)
            .HasForeignKey(bi => bi.BoardListId);
    }
}
