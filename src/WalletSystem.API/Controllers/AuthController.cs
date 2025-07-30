using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WalletSystem.Core.Application.DTOs;

namespace WalletSystem.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config) => _config = config;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        if (dto.User != "admin" || dto.Password != "123456")
            return Unauthorized();
        var token = GenerateToken(dto.User);
        return Ok(new { token });
    }

    private string GenerateToken(string user)
    {
        var key = Encoding.ASCII.GetBytes("claveUltraSecretaDe32Caracteres12345678");
        var token = new JwtSecurityToken(
            claims: [new Claim(ClaimTypes.Name, user)],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}