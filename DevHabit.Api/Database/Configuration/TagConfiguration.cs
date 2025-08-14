using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Database.Configuration;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasOne<User>().WithMany().HasForeignKey(h => h.UserId).OnDelete(DeleteBehavior.NoAction);
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasMaxLength(500);
        builder.Property(t => t.Name).HasMaxLength(50);
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.UserId).HasMaxLength(500);
        builder.HasIndex(t => new { t.Name, t.UserId }).IsUnique();
    }
}