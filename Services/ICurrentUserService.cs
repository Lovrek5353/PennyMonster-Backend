using System.Security.Claims;

namespace PennyMonster.Services
{
    public interface ICurrentUserService
    {
        string? FirebaseUid { get; }
        ClaimsPrincipal? Principal { get; }
    }
}
