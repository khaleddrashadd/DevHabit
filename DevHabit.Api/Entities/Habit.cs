namespace DevHabit.Api.Entities;

public sealed class Habit
{
    public required string Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required HabitType Type { get; set; }
    public required Frequency Frequency { get; set; }
    public required Target Target { get; set; }
    public required HabitStatus Status { get; set; }
    public required bool IsArchived { get; set; }
    public DateOnly? EndDate { get; set; } // Optional end date for the habit
    public Milestone? Milestone { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? LastCompletedAtUtc { get; set; }
}

public sealed class Milestone
{
    public int Target { get; set; } // e.g., 10, 100, etc.

    public int Current { get; set; } // e.g., 5, 50, etc.
    // when Current == Target.Value, the habit is considered completed
}

public enum HabitStatus
{
    None = 0,
    Ongoing = 1,
    Completed = 2
}

public sealed class Target
{
    public required string Unit { get; set; }
    public required int Value { get; set; } // e.g., 10, 100, etc.
}

public enum HabitType
{
    None = 0,
    Binary = 1,
    Measurable = 2
}

public sealed class Frequency
{
    public required FrequencyType Type { get; set; }
    public required int TimePerPeriod { get; set; } // e.g., 30 minutes, 1 hour, etc.
}

public enum FrequencyType
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3
}