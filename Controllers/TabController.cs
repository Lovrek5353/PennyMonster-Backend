using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyMonster.DTOs;
using PennyMonster.Enums;
using PennyMonster.Services;

namespace PennyMonster.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TabController(ITabService tabService, IUserService currentUser): ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTabs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null
            )
        {
            if (pageSize > 100) pageSize = 100;
            if (pageNumber < 1) pageNumber = 1;

            var activeUserId=await currentUser.GetUserIdAsync();
            if (activeUserId == Guid.Empty) return Unauthorized();

            var (tabItems, totalCount) = await tabService.GetTabsAsync(activeUserId, pageNumber, pageSize, startDate, endDate);

            return Ok(new
            {
                Items = tabItems,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTab([FromBody] TabCreateDto tab)
        {
            var activeUserId = await currentUser.GetUserIdAsync();
            if (activeUserId == Guid.Empty) return Unauthorized();

            var result=await tabService.CreateTabAsync(activeUserId, tab);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTab(Guid id)
        {
            var activeUserId = await currentUser.GetUserIdAsync();
            if (activeUserId == Guid.Empty) return Unauthorized();

            var wasDeleted=await tabService.DeleteTabAsync(activeUserId, id);

            if (!wasDeleted)
            {
                return NotFound("Tab not found or you don't have permission to delete it.");
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTab(Guid id, [FromBody] TabUpdateDto dto)
        {
            var activeUserId = await currentUser.GetUserIdAsync();
            if (activeUserId == Guid.Empty) return Unauthorized();

            var updatedTab = await tabService.UpdateTabAsync(activeUserId, id, dto);

            if (updatedTab == null)
            {
                return NotFound("Tab not found or permission denied.");
            }

            return Ok(updatedTab);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTabById(Guid id)
        {
            var activeUserId = await currentUser.GetUserIdAsync();
            if (activeUserId == Guid.Empty) return Unauthorized();

            var tab= await tabService.GetTabByIdAsync(activeUserId, id);
            if (tab == null)
            {
                return NotFound($"Tab with ID {id} was not found for this user.");
            }
            return Ok(tab);
        }
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] TabStatus newStatus)
        {
            var activeUserId = await currentUser.GetUserIdAsync();
            if (activeUserId == Guid.Empty) return Unauthorized();
            var success = await tabService.UpdateTabStatusAsync(activeUserId, id, newStatus);

            if (!success)
                return BadRequest("Invalid status transition or Tab not found.");

            return NoContent();
        }
    }
}
