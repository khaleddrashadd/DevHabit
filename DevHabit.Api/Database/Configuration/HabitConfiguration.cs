using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Database.Configuration;

public class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.ToTable("Habits", Schemas.Application);

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id).HasMaxLength(500);

        builder.Property(h => h.Description).HasMaxLength(500);

        builder.OwnsOne(h => h.Frequency);
        builder.OwnsOne(h => h.Target, target => { target.Property(t => t.Unit).HasMaxLength(100); });


        builder.OwnsOne(h => h.Milestone);

        builder.OwnsOne(h => h.Target, target => { target.Property(t => t.Unit).HasMaxLength(100); });
    }
}