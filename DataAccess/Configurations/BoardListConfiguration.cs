using ArchivingFunction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArchivingFunction.Persistence.Configurations;

public class BoardListConfiguration : BaseEntityConfiguration<BoardList>
{
    public override void Configure(EntityTypeBuilder<BoardList> builder)
    {
        base.Configure(builder);

        builder.ToTable("BoardLists");

        builder.Property(bl => bl.Title).IsRequired();

        builder.HasMany(bl => bl.BoardItems)
            .WithOne()
            .HasForeignKey(bi => bi.BoardListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
