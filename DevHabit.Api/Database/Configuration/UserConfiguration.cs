using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Database.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasMaxLength(500);
        builder.Property(u => u.Name).HasMaxLength(500);
        builder.Property(u => u.Email).HasMaxLength(500);
        builder.Property(u => u.IdentityId).HasMaxLength(500);
        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasIndex(user => user.IdentityId).IsUnique();
    }
}