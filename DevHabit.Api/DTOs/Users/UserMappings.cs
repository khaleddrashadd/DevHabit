using DevHabit.Api.DTOs.Auth;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace DevHabit.Api.DTOs.Users;

public static class UserMappings
{
    public static User ToEntity(this RegisterRequestDto registerDto)
    {
        var user = new User
        {
            Id = $"u_{Guid.CreateVersion7()}",
            Email = registerDto.Email,
            Name = registerDto.Name,
            CreatedAtUtc = DateTime.UtcNow
        };
        return user;
    }
}