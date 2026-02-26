using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Persistence.Configurations;

public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.ToTable("Boards");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired();

        builder.Property(b => b.Visibility)
            .IsRequired();

        builder.HasMany(b => b.BoardLists)
            .WithOne(bl => bl.Board)
            .HasForeignKey(bl => bl.BoardId);

        builder.OwnsOne(b => b.PermissionRoles, settings =>
        {
            settings.Property(s => s.MinCreateItemRole)
                .HasColumnName("MinCreateItemRole");

            settings.Property(s => s.MinChangeItemRole)
                .HasColumnName("MinChangeItemRole");

            settings.Property(s => s.MinCreateListRole)
                .HasColumnName("MinCreateListRole");

            settings.Property(s => s.MinChangeListRole)
                .HasColumnName("MinChangeListRole");
        });
        
        builder.Property(b => b.ArchiveStatus)
            .IsRequired();
    }
}