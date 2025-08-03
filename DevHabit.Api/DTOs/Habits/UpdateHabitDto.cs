using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;

namespace DevHabit.Api.DTOs;

public sealed record UpdateHabitDto(
    string Id,
    string Name,
    string? Description,
    HabitType Type,
    FrequencyDto Frequency,
    TargetDto Target,
    DateOnly? EndDate,
    UpdateMilestoneDto? Milestone
);

public sealed record UpdateMilestoneDto(int Target);