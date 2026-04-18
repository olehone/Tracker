using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class RoadmapNodeConfiguration : IEntityTypeConfiguration<RoadmapNode>
{
    public void Configure(EntityTypeBuilder<RoadmapNode> builder)
    {
        builder.ToTable("RoadmapNodes");

        builder.HasKey(n => n.Id);

        builder.HasIndex(n => new { n.BoardId, n.BoardItemId })
            .IsUnique();

        builder.HasOne(n => n.Board)
            .WithMany()
            .HasForeignKey(n => n.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.BoardItem)
            .WithMany()
            .HasForeignKey(n => n.BoardItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
