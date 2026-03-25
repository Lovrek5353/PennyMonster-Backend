using PennyMonster.DTOs;
using PennyMonster.Enums;

namespace PennyMonster.Services
{
    public interface ITabService
    {
        Task<(IEnumerable<TabDto> Tabs, int TotalCount)> GetTabsAsync(
            Guid userId, 
            int pageNumber, 
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null
            );
        Task<TabDto> CreateTabAsync(Guid userId, TabCreateDto dto);
        Task<bool> DeleteTabAsync(Guid userId, Guid id);
        Task <TabDto?> UpdateTabAsync(Guid userId, Guid id, TabUpdateDto tabDto);
        Task <TabDto?> GetTabByIdAsync(Guid userId, Guid id);
        Task <bool> UpdateTabStatusAsync(Guid userId, Guid id, TabStatus newStatus);
    }
}
