using PennyMonster.DTOs;

namespace PennyMonster.Services
{
    public interface IAuthService
    {
        string GenerateMockToken(TokenRequestDto request);
    }
}
