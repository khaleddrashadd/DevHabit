using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.DTOs.Habits;

public sealed record HabitQueryParamsDto
{
    [FromQuery(Name = "q")] public string Search { get; set; } = "";
    public HabitType? Type { get; init; }
    public HabitStatus? Status { get; init; }
    public string? Sort { get; init; }

    public void Deconstruct(out string search, out HabitType? type, out HabitStatus? status, out string sort)
    {
        search = Search.ToLowerInvariant();
        type = Type;
        status = Status;
        sort = Sort?.ToLowerInvariant() ?? string.Empty;
    }
}