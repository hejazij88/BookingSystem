using BookingSystem.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookingSystem.Applications.JWT;

public interface IJwtTokenGenerator
{
    Task<(string AccessToken, RefreshToken RefreshToken)> GenerateTokensAsync(ApplicationUser user, string ipAddress);
    string GenerateJwtToken(ApplicationUser user);
    RefreshToken GenerateRefreshToken(string ipAddress);

}

public class JwtTokenGenerator : IJwtTokenGenerator
{

    private readonly JwtSettings _settings;


    public JwtTokenGenerator(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<(string AccessToken, RefreshToken RefreshToken)> GenerateTokensAsync(ApplicationUser user, string ipAddress)
    {
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken(ipAddress);
        return (accessToken, refreshToken);
    }

    public string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.FirstName),

        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }



        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(string ipAddress)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);

        return new RefreshToken
        {
            Token = token,
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }
}