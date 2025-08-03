using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;

namespace DevHabit.Api.DTOs;

public sealed record CreateHabitDto(
    string Name,
    string? Description,
    HabitType Type,
    FrequencyDto Frequency,
    TargetDto Target,
    DateOnly? EndDate,
    MilestoneDto? Milestone
);