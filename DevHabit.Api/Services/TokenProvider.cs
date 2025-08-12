using System.Security.Claims;
using System.Text;
using DevHabit.Api.DTOs.Auth;
using DevHabit.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace DevHabit.Api.Services;

public sealed class TokenProvider(IOptions<JwtAuthOptions> options)
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public AccessTokenDto Create(TokenRequest request)
    {
        return new AccessTokenDto(GenerateAccessToken(request), GenerateRefreshToken());
    }

    private string GenerateAccessToken(TokenRequest request)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, request.Email),
            new(JwtRegisteredClaimNames.Sub, request.UserId)
        };
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
        var securityCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        // var token = new JwtSecurityToken(_jwtAuthOptions.Issuer, _jwtAuthOptions.Audience,
        //     expires: DateTime.Now.AddMinutes(_jwtAuthOptions.ExpirationInMinutes),
        //     signingCredentials: securityCredential);
        // return new JwtSecurityTokenHandler().WriteToken(token);
        /////////////////////////////////////////////////////////////////////
        //this is the new way to create a JWT token using Microsoft.IdentityModel.JsonWebTokens
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes),
            Issuer = _jwtAuthOptions.Issuer,
            Audience = _jwtAuthOptions.Audience,
            SigningCredentials = securityCredential
        };
        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }

    private string GenerateRefreshToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
        var securityCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationInDays),
            Issuer = _jwtAuthOptions.Issuer,
            Audience = _jwtAuthOptions.Audience,
            SigningCredentials = securityCredential
        };
        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }
}