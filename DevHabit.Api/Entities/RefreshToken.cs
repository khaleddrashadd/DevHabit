using Microsoft.AspNetCore.Identity;

namespace DevHabit.Api.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public required string Token { get; set; }
    public required string UserId { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    //this Navigation because I want to use refresh token as part of Identity db context
    public IdentityUser User { get; set; } = null!;
}