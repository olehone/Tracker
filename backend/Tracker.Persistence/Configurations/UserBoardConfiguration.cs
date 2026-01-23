using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class UserBoardConfiguration : IEntityTypeConfiguration<UserBoard>
{
    public void Configure(EntityTypeBuilder<UserBoard> builder)
    {
        builder.ToTable("UserBoards");

        builder.HasKey(x => x.Id);

        builder.Property(ub => ub.Role)
            .IsRequired();

        builder.HasOne(ub => ub.User)
            .WithMany(u => u.UserBoards)
            .HasForeignKey(ub => ub.UserId);

        builder.HasOne(ub => ub.Board)
            .WithMany(u => u.UserBoards)
            .HasForeignKey(ub => ub.BoardId);

        builder.HasIndex(x => new { x.UserId, x.BoardId })
               .IsUnique();
    }
}