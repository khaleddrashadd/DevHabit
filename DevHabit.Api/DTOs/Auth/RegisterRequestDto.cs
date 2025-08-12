namespace DevHabit.Api.DTOs.Auth;

public sealed record RegisterRequestDto
{
    public required string UserName { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
};