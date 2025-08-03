namespace DevHabit.Api.DTOs.Habits;

public sealed record HabitsCollectionDto
{
    public List<HabitDto> Items { get; init; } = [];
};