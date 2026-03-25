using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyMonster.DTOs;
using PennyMonster.Models;
using PennyMonster.Services;

namespace PennyMonster.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService, IUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var activeUserId = await currentUser.GetUserIdAsync();

        if (activeUserId == Guid.Empty)
        {
            return Unauthorized("User profile not synced.");
        }

        var categories = await categoryService.GetCategoriesAsync(activeUserId);
        return Ok(categories);
    }

    // POST: api/categories
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto categoryDto)
    {
        var activeUserId= await currentUser.GetUserIdAsync();
        if (activeUserId == Guid.Empty) return Unauthorized();

        var result = await categoryService.CreateCategoryAsync(activeUserId, categoryDto);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // DELETE: api/categories/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var activeUserId = await currentUser.GetUserIdAsync();
        if (activeUserId == Guid.Empty) return Unauthorized();

        var wasDeleted = await categoryService.DeleteCategoryAsync(id, activeUserId);

        if (!wasDeleted)
        {
            return NotFound("Category not found or you don't have permission to delete it.");
        }
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryUpdateDto dto)
    {
        var activeUserId = await currentUser.GetUserIdAsync();
        if (activeUserId == Guid.Empty) return Unauthorized();

        var updatedCategory = await categoryService.UpdateCategoryAsync(id, activeUserId, dto);

        if (updatedCategory == null)
        {
            return NotFound("Category not found or you do not have permission to edit it.");
        }

        return Ok(updatedCategory);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var activeUserId = await currentUser.GetUserIdAsync();
        if (activeUserId == Guid.Empty) return Unauthorized();

        var category = await categoryService.GetCategoryByIdAsync(id, activeUserId);

        if (category == null)
        {
            return NotFound($"Category with ID {id} was not found for this user.");
        }

        return Ok(category);
    }
}