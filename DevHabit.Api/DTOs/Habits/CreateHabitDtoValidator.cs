using DevHabit.Api.Entities;
using FluentValidation;

namespace DevHabit.Api.DTOs.Habits;

public sealed class CreateHabitDtoValidator : AbstractValidator<CreateHabitDto>
{
    private static readonly string[] AllowedUnits =
        ["minutes", "hours", "steps", "km", "cal", "pages", "books", "tasks", "sessions"];

    private static readonly string[] AllowedUnitsForBinaryHabits =
        ["tasks", "sessions"];

    public CreateHabitDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(100)
            .WithMessage("Habit name must be between 3 and 100 characters.");
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null)
            .WithMessage("Description must be at most 500 characters.");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid habit type specified.");
        RuleFor(x => x.Frequency.Type).IsInEnum().WithMessage("Invalid frequency period.");
        RuleFor(x => x.Frequency.TimePerPeriod).GreaterThan(0).WithMessage("Time per period must be greater than 0.");
        RuleFor(x => x.Target.Value).GreaterThan(0).WithMessage("Target value must be greater than 0.");
        RuleFor(x => x.Target.Unit).NotEmpty().WithMessage("Target unit must not be empty.")
            .Must((unit) => AllowedUnits.Contains(unit.ToLowerInvariant()))
            .WithMessage($"Target unit must be one of the following: {string.Join(", ", AllowedUnits)}.");
        RuleFor(x => x.EndDate).GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be in the future.");
        When(x => x.Milestone is not null,
            () =>
            {
                RuleFor(x => x.Milestone!.Target).GreaterThan(0)
                    .WithMessage("Milestone target must be greater than 0.");
            });

        RuleFor(x => x.Target.Unit).Must((dto, unit) => IsTargetUnitAllowedWithType(dto.Type, unit))
            .WithMessage("Target unit is not allowed for the specified habit type.")
            ;
    }

    private static bool IsTargetUnitAllowedWithType(HabitType type, string unit)
    {
        return type switch
        {
            HabitType.Binary => AllowedUnitsForBinaryHabits.Contains(unit.ToLowerInvariant()),
            HabitType.Measurable => AllowedUnits.Contains(unit.ToLowerInvariant()),
            _ => false
        };
    }
};