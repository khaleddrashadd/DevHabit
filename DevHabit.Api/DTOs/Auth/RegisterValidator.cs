using FluentValidation;

namespace DevHabit.Api.DTOs.Auth;

public sealed class RegisterValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress()
            .WithMessage("Invalid email format.");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password is required.")
            .MinimumLength(6).WithMessage("Confirm Password must be at least 6 characters long.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}