using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.ToTable("Boards");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired();

        builder.HasMany(b => b.BoardLists)
            .WithOne(bl => bl.Board)
            .HasForeignKey(bl => bl.BoardId);
    }
}