using Microsoft.EntityFrameworkCore;
using PennyMonster.Data;
using PennyMonster.DTOs;
using PennyMonster.Models;
using System.Security.Claims;

namespace PennyMonster.Services
{
    public class UserService(PennyMonsterContext context, ICurrentUserService currentUserService) : IUserService
    {
        public async Task<Guid> GetUserIdAsync()
        {
            var firebaseUid = currentUserService.FirebaseUid;
            if (string.IsNullOrEmpty(firebaseUid)) return Guid.Empty;

            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

            return user?.Id ?? Guid.Empty;
        }

        public async Task<UserSyncDto> SyncUserAsync()
        {
            var firebaseUid = currentUserService.FirebaseUid;
            if (string.IsNullOrEmpty(firebaseUid))
                throw new UnauthorizedAccessException("UID not found in token.");

            var principal = currentUserService.Principal;
            var email = principal?.FindFirstValue(ClaimTypes.Email) ?? "no-email@test.com";
            var firstName = principal?.FindFirstValue("given_name") ?? "New";
            var lastName = principal?.FindFirstValue("family_name") ?? "User";

            var user = await context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    FirebaseUid = firebaseUid,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Username = email.Split('@')[0],
                    CreatedAtUtc = DateTime.UtcNow
                };
                context.Users.Add(user);
            }
            else
            {
                user.Email = email;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.LastModified = DateTime.UtcNow;
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                context.Entry(user).State = EntityState.Detached;
                user = await context.Users.AsNoTracking().FirstAsync(u => u.FirebaseUid == firebaseUid);
            }

            return new UserSyncDto
            {
                InternalId = user.Id,
                ExternalId = user.FirebaseUid,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}"
            };
        }
    }
}
