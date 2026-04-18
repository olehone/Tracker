using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class RoadmapArrowConfiguration : IEntityTypeConfiguration<RoadmapArrow>
{
    public void Configure(EntityTypeBuilder<RoadmapArrow> builder)
    {
        builder.ToTable("RoadmapArrows");

        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Source)
            .WithMany(n => n.OutgoingArrows)
            .HasForeignKey(a => a.SourceNodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Target)
            .WithMany(n => n.IncomingArrows)
            .HasForeignKey(a => a.TargetNodeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
