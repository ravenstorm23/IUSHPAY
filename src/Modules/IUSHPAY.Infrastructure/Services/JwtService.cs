using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IUSHPAY.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key no configurada");
        var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer no configurado");
        var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience no configurado");
        var expiresMinutes = double.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("code", user.InstitutionalCode),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
