using ArchivingFunction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArchivingFunction.Persistence.Configurations;

public class BoardConfiguration : BaseEntityConfiguration<Board>
{
    public override void Configure(EntityTypeBuilder<Board> builder)
    {
        base.Configure(builder);

        builder.ToTable("Boards");

        builder.Property(b => b.Title).IsRequired();
        builder.Property(b => b.Visibility).IsRequired();
        builder.Property(b => b.ArchiveStatus).IsRequired();

        builder.OwnsOne(b => b.PermissionRoles, owned =>
        {
            owned.Property(p => p.MinCreateItemRole).HasColumnName("MinCreateItemRole");
            owned.Property(p => p.MinChangeItemRole).HasColumnName("MinChangeItemRole");
            owned.Property(p => p.MinCreateListRole).HasColumnName("MinCreateListRole");
            owned.Property(p => p.MinChangeListRole).HasColumnName("MinChangeListRole");
        });

        builder.HasMany(b => b.BoardLists)
            .WithOne()
            .HasForeignKey(bl => bl.BoardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}