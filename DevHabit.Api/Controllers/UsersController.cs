using System.Security.Claims;
using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Users;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController(ApplicationDbContext dbContext, UserContext userContext) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById([FromRoute] string id)
    {
        var currentUserId = await userContext.GetUserAsync();
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized("User not authenticated");

        if (currentUserId != id)
            return Forbid();

        var user = await dbContext.Users.Where(u => u.Id == id).Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var currentUserId = await userContext.GetUserAsync();
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized("User not authenticated");
        var user = await dbContext.Users.Where(u => u.Id == currentUserId).Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();
        if (user == null)
            return NotFound("User not found");

        return Ok(user);
    }
}