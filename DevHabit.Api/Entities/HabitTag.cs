namespace DevHabit.Api.Entities;

public class HabitTag
{
    public required string HabitId { get; init; }
    public required string TagId { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}