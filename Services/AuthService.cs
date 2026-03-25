using Microsoft.IdentityModel.Tokens;
using PennyMonster.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PennyMonster.Services
{
    public class AuthService(IConfiguration config) : IAuthService
    {
        public string GenerateMockToken(TokenRequestDto request)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, request.FirebaseUid),
            new Claim(ClaimTypes.Email, request.Email),
            new Claim("given_name", request.FirstName),
            new Claim("family_name", request.LastName)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
