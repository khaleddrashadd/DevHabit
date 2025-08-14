namespace DevHabit.Api.Entities;

public sealed class Tag
{
    public required string Id { get; init; }
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ICollection<HabitTag> HabitTags { get; set; } = [];
    public ICollection<Habit> Habits { get; set; } = [];
}