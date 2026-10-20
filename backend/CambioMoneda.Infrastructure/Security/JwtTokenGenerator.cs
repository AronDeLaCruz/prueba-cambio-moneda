using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CambioMoneda.Application.Dtos;
using CambioMoneda.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CambioMoneda.Infrastructure.Security;

public class JwtTokenGenerator(IConfiguration config) : IJwtTokenGenerator
{
    public LoginResponse Generate(string usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var expira = DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:ExpireMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: [new Claim(ClaimTypes.Name, usuario)],
            expires: expira,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}