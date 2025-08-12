using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Auth;
using DevHabit.Api.DTOs.Users;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class AuthController(
    UserManager<IdentityUser> userManager,
    ApplicationDbContext dbContext,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        await using var transaction = await identityDbContext.Database.BeginTransactionAsync();

        // we need to set the connection of the main db context to the identity db context
        // so that we can use the same transaction for both contexts
        // this is necessary because the userManager operates on the identityDbContext
        // and we want to ensure that the user creation and the user entity creation are part of the same transaction.

        dbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        var identityUser = new IdentityUser
        {
            UserName = registerRequestDto.UserName,
            Email = registerRequestDto.Email
        };

        var result = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
        if (!result.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "Errors", result.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };
            return Problem(
                "User registration failed",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }

        var userEntity = registerRequestDto.ToEntity();
        userEntity.IdentityId = identityUser.Id;

        await dbContext.AddAsync(userEntity);
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email);
        var accessToken = tokenProvider.Create(tokenRequest);
        return Ok(accessToken);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenDto>> Login([FromBody] LoginRequestDto loginRequest)
    {
        var identityUser = await userManager.FindByEmailAsync(loginRequest.Email);
        if (identityUser == null || !await userManager.CheckPasswordAsync(identityUser, loginRequest.Password))
            return Unauthorized("Invalid username or password");

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!);
        var accessToken = tokenProvider.Create(tokenRequest);
        return Ok(accessToken);
    }
}