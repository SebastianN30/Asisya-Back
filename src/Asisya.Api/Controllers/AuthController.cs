using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Asisya.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  [HttpPost("login")]
  public IActionResult Login([FromBody] LoginRequest request, [FromServices] IConfiguration config)
  {
    if (request.Username != "admin" || request.Password != "admin")
      return Unauthorized();

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: new[] { new Claim(ClaimTypes.Name, request.Username) },
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds);

    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
  }
}
public record LoginRequest(string Username, string Password);
