using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Title)
            .IsRequired();

        builder.HasMany(w => w.Boards)
            .WithOne(w => w.Workspace)
            .HasForeignKey(b => b.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(w => w.Settings, settings =>
        {
            settings.Property(s => s.Visibility)
                .HasColumnName("Visibility");

            settings.Property(s => s.MinCreateBoardRole)
                .HasColumnName("MinCreateBoardRole");

            settings.Property(s => s.MinChangeBoardRole)
                .HasColumnName("MinChangeBoardRole");
        });
    }
}