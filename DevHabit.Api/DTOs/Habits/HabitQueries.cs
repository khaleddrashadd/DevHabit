using DevHabit.Api.Entities;
using System.Linq.Expressions;

namespace DevHabit.Api.DTOs.Habits;

public static class HabitQueries
{
    public static Expression<Func<Habit, HabitDto>> ProjectToDto()
    {
        return h => new HabitDto(
            h.Id,
            h.Name,
            h.Description,
            h.Type,
            new FrequencyDto(h.Frequency.Type,
                h.Frequency.TimePerPeriod),
            new TargetDto(h.Target.Value,
                h.Target.Unit)
            ,
            h.Status,
            h.IsArchived,
            h.EndDate,
            h.Milestone != null
                ? new MilestoneDto
                (
                    h.Milestone.Current,
                    h.Milestone.Target
                )
                : null
            ,
            h.CreatedAtUtc,
            h.UpdatedAtUtc, h.LastCompletedAtUtc
        );
    }
}