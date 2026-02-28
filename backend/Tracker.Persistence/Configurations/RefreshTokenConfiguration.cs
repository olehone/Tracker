using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions");

        builder.HasKey(subscription => subscription.Id);

        builder.HasOne(subscription => subscription.User)
            .WithOne()
            .HasForeignKey<UserSubscription>(subscription => subscription.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
