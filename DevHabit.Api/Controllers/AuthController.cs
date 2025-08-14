using System.Security.Claims;
using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Auth;
using DevHabit.Api.DTOs.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using DevHabit.Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class AuthController(
    UserManager<IdentityUser> userManager,
    ApplicationDbContext dbContext,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options)
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

        var identityResult = await userManager.AddToRoleAsync(identityUser, Roles.Member);
        if (!identityResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "Errors", identityResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
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

        var userRoles = await userManager.GetRolesAsync(identityUser);

        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email, userRoles
        );
        var tokenDto = tokenProvider.Create(tokenRequest);
        await CreateRefreshTokenAsync(identityUser, tokenDto);

        await transaction.CommitAsync();

        return Ok();
    }

    private async Task CreateRefreshTokenAsync(IdentityUser identityUser, AccessTokenDto tokenDto)
    {
        var refreshToken = new RefreshToken
        {
            UserId = identityUser.Id,
            Token = tokenDto.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(options.Value.RefreshTokenExpirationInDays),
            Id = Guid.CreateVersion7()
        };

        await identityDbContext.RefreshTokens.AddAsync(refreshToken);
        await identityDbContext.SaveChangesAsync();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenDto>> Login([FromBody] LoginRequestDto loginRequest)
    {
        var identityUser = await userManager.FindByEmailAsync(loginRequest.Email);
        if (identityUser == null || !await userManager.CheckPasswordAsync(identityUser, loginRequest.Password))
            return Unauthorized("Invalid username or password");

        var userRoles = await userManager.GetRolesAsync(identityUser);
        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!, userRoles);
        var tokenDto = tokenProvider.Create(tokenRequest);
        await CreateRefreshTokenAsync(identityUser, tokenDto);
        return Ok(tokenDto);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AccessTokenDto>> Refresh([FromBody] RefreshTokenDto refreshTokenRequest)
    {
        var refreshToken = await identityDbContext.RefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenRequest.RefreshToken);

        if (refreshToken == null || refreshToken.ExpiresAtUtc < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");
        var userRoles = await userManager.GetRolesAsync(refreshToken.User);
        var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!, userRoles);
        var tokenDto = tokenProvider.Create(tokenRequest);

        // Update the existing refresh token
        refreshToken.Token = tokenDto.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(options.Value.RefreshTokenExpirationInDays);
        await identityDbContext.SaveChangesAsync();

        return Ok(tokenDto);
    }
}