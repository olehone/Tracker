using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class BoardItemConfiguration : IEntityTypeConfiguration<BoardItem>
{
    public void Configure(EntityTypeBuilder<BoardItem> builder)
    {
        builder.ToTable("BoardItems");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired();
    }
}