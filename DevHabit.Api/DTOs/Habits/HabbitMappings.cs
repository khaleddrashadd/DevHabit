using DevHabit.Api.Entities;

namespace DevHabit.Api.DTOs.Habits;

public static class HabitMappings
{
    public static Habit ToEntity(this CreateHabitDto dto)
    {
        Habit habit = new()
        {
            Id = $"h_{Guid.CreateVersion7()}",
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type,
            Frequency = new Frequency
            {
                Type = dto.Frequency.Type,
                TimePerPeriod = dto.Frequency.TimePerPeriod
            },
            Target = new Target
            {
                Value = dto.Target.Value,
                Unit = dto.Target.Unit
            },
            Status = HabitStatus.Ongoing,
            IsArchived = false,
            CreatedAtUtc = DateTime.UtcNow,
            EndDate = dto.EndDate,
            Milestone = dto.Milestone is not null
                ? new Milestone
                {
                    Current = 0,
                    Target = dto.Milestone.Target
                }
                : null
        };

        return habit;
    }

    public static HabitDto ToDto(this Habit habit)
    {
        return new HabitDto
        (
            habit.Id,
            habit.Name,
            habit.Description,
            habit.Type,
            new FrequencyDto(habit.Frequency.Type, habit.Frequency.TimePerPeriod),
            new TargetDto(habit.Target.Value, habit.Target.Unit),
            habit.Status,
            habit.IsArchived,
            habit.EndDate,
            habit.Milestone is not null ? new MilestoneDto(habit.Milestone.Current, habit.Milestone.Target) : null,
            habit.CreatedAtUtc,
            habit.UpdatedAtUtc,
            habit.LastCompletedAtUtc
        );
    }

    public static void UpdateFromDto(this Habit habit, UpdateHabitDto dto)
    {
        habit.Name = dto.Name;
        habit.Description = dto.Description;
        habit.Type = dto.Type;
        habit.EndDate = dto.EndDate;
        habit.Frequency = new Frequency
        {
            Type = dto.Frequency.Type,
            TimePerPeriod = dto.Frequency.TimePerPeriod
        };
        habit.Target = new Target
        {
            Value = dto.Target.Value,
            Unit = dto.Target.Unit
        };
        habit.UpdatedAtUtc = DateTime.UtcNow;

        if (dto.Milestone is null) return;
        habit.Milestone ??= new Milestone();
        habit.Milestone.Target = dto.Milestone.Target;
    }
}