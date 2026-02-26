using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArchivingFunction.Domain.Entities;

namespace ArchivingFunction.Persistence.Configurations;

public class ItemCommentConfiguration : IEntityTypeConfiguration<ItemComment>
{
    public void Configure(EntityTypeBuilder<ItemComment> builder)
    {
        builder.ToTable("ItemComments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .IsRequired();
    }
}