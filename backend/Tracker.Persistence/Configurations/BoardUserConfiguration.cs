using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class BoardUserConfiguration : IEntityTypeConfiguration<BoardUser>
{
    public void Configure(EntityTypeBuilder<BoardUser> builder)
    {
        builder.ToTable("BoardUsers");

        builder.HasKey(x => x.Id);

        builder.Property(ub => ub.Role)
            .IsRequired();

        builder.HasOne(ub => ub.User)
            .WithMany(u => u.BoardUsers)
            .HasForeignKey(ub => ub.UserId);

        builder.HasOne(ub => ub.Board)
            .WithMany(u => u.BoardUsers)
            .HasForeignKey(ub => ub.BoardId);

        builder.HasIndex(x => new { x.UserId, x.BoardId })
               .IsUnique();
    }
}