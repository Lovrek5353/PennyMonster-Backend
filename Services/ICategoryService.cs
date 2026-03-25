using PennyMonster.DTOs;
using PennyMonster.Models;

namespace PennyMonster.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(Guid userId);
        Task<CategoryDto> CreateCategoryAsync(Guid userId, CategoryCreateDto dto);
        Task<bool> DeleteCategoryAsync(Guid id, Guid userId);
        Task<CategoryDto?> UpdateCategoryAsync(Guid id, Guid userId, CategoryUpdateDto dto);
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id, Guid userId);
    }
}
