using DevHabit.Api.Entities;

namespace DevHabit.Api.DTOs.Habits;

public sealed record HabitDto(
    string Id,
    string Name,
    string? Description,
    HabitType Type,
    FrequencyDto Frequency,
    TargetDto Target,
    HabitStatus Status,
    bool IsArchived,
    DateOnly? EndDate = null,
    MilestoneDto? Milestone = null,
    DateTime CreatedAtUtc = default,
    DateTime? UpdatedAtUtc = null,
    DateTime? LastCompletedAtUtc = null);