using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PennyMonster.DTOs;
using PennyMonster.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PennyMonster.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("get-test-token")]
    public IActionResult GetToken([FromBody] TokenRequestDto request)
    {
        var token = authService.GenerateMockToken(request);
        return Ok(new TokenResponseDto { Token = token });
    }
}