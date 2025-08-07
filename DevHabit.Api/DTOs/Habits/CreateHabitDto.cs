using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;

namespace DevHabit.Api.DTOs.Habits;

public sealed record CreateHabitDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required HabitType Type { get; set; }
    public required FrequencyDto Frequency { get; set; }
    public required TargetDto Target { get; set; }
    public DateOnly? EndDate { get; set; }
    public required MilestoneDto? Milestone { get; set; }
};