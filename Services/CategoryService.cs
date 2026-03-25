using Microsoft.EntityFrameworkCore;
using PennyMonster.Data;
using PennyMonster.DTOs;
using PennyMonster.Models;

namespace PennyMonster.Services;

public class CategoryService(PennyMonsterContext context) : ICategoryService
{
    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(Guid userId)
    {
        var categories = await context.Categories
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Color = c.Color,
            CreatedAtUtc = c.CreatedAtUtc,
            LastModified = c.LastModified
        });
    }

    public async Task<CategoryDto> CreateCategoryAsync(Guid userId, CategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            Color = dto.Color,
            UserId = userId
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            CreatedAtUtc = category.CreatedAtUtc,
            LastModified = category.LastModified
        };
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, Guid userId)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
        {
            return false;
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(Guid id, Guid userId, CategoryUpdateDto dto)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null) return null;

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.Color = dto.Color;

        await context.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            CreatedAtUtc = category.CreatedAtUtc,
            LastModified = category.LastModified
        };
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, Guid userId)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Color = category.Color,
            CreatedAtUtc = category.CreatedAtUtc,
            LastModified = category.LastModified
        };
    }
}