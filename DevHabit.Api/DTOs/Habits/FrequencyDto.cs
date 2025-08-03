using DevHabit.Api.Entities;

namespace DevHabit.Api.DTOs.Habits;

public sealed record FrequencyDto(
    FrequencyType Type,
    int TimePerPeriod);