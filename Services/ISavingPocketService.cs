using Microsoft.Extensions.Configuration.UserSecrets;
using PennyMonster.DTOs;
using PennyMonster.Enums;

namespace PennyMonster.Services
{
    public interface ISavingPocketService
    {
        Task<(IEnumerable<SavingPocketDto> Items, int TotalCount)> GetSavingPocketsAsync(
            Guid userId,
            int page=1,
            int pageSize=20,
            Guid? pocketId=null,
            DateTime? startDate=null,
            DateTime? endDate=null,
            List <Guid>? pocketIds= null
            );
        Task<SavingPocketDto> CreateSavingPocketAsync(Guid userId, SavingPocketCreateDto dto);
        Task<bool> DeleteSavingPocketAsync(Guid userId, Guid pocketId);
        Task<SavingPocketDto?> UpdateSavingPocketAsync(Guid userId, Guid id, SavingPocketUpdateDto dto);
        Task<SavingPocketDto?> GetSavingPocketByIdAsync(Guid userId, Guid pocketId);
        Task<bool> UpdateSavingPocketStatusAsync(Guid userId, Guid id, SavingPocketStatus newStatus);
    }
}
