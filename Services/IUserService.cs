using PennyMonster.DTOs;
using System.Security.Claims;

namespace PennyMonster.Services
{
    public interface IUserService
    {
        Task<UserSyncDto> SyncUserAsync();

        Task<Guid> GetUserIdAsync();
    }
}
