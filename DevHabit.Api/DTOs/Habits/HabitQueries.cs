using DevHabit.Api.Entities;
using System.Linq.Expressions;

namespace DevHabit.Api.DTOs.Habits;

public static class HabitQueries
{
    public static Expression<Func<Habit, HabitDto>> ProjectToDto()
    {
        return h => new HabitDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Type = h.Type,
            Frequency = new FrequencyDto(h.Frequency.Type,
                h.Frequency.TimePerPeriod),
            Target = new TargetDto(h.Target.Value,
                h.Target.Unit),
            Status = h.Status,
            IsArchived = h.IsArchived,
            EndDate = h.EndDate,
            Milestone = h.Milestone != null
                ? new MilestoneDto
                (
                    h.Milestone.Current,
                    h.Milestone.Target
                )
                : null,

            CreatedAtUtc = h.CreatedAtUtc,
            UpdatedAtUtc = h.UpdatedAtUtc
        };
    }

    public static Expression<Func<Habit, HabitWithTagsDto>> ProjectWithTagsToDto()
    {
        return h => new HabitWithTagsDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Type = h.Type,
            Frequency = new FrequencyDto(h.Frequency.Type,
                h.Frequency.TimePerPeriod),
            Target = new TargetDto(h.Target.Value,
                h.Target.Unit),
            Status = h.Status,
            IsArchived = h.IsArchived,
            EndDate = h.EndDate,
            Milestone = h.Milestone != null
                ? new MilestoneDto
                (
                    h.Milestone.Current,
                    h.Milestone.Target
                )
                : null,
            Tags = h.Tags.Select(t => t.Id).ToList(),
            CreatedAtUtc = h.CreatedAtUtc,
            UpdatedAtUtc = h.UpdatedAtUtc
        };
    }
}