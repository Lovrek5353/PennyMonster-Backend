using Microsoft.EntityFrameworkCore;
using PennyMonster.Data;
using System.Security.Claims;

namespace PennyMonster.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

        public string? FirebaseUid => Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
