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
        public class SavingPocketController(ISavingPocketService savingPocketService, IUserService currentUser) : ControllerBase
        {
            [HttpGet]
            public async Task<IActionResult> GetSavingPockets
                (
                    [FromQuery] int page = 1,
                    [FromQuery] int pageSize = 20,
                    [FromQuery] Guid? pocketId = null,
                    [FromQuery] DateTime? startDate = null,
                    [FromQuery] DateTime? endDate = null,
                    [FromQuery] List<Guid>? pocketIds = null
                )
            {
                if (pageSize > 100) pageSize = 100;
                if (page < 1) page = 1;

                var activeUserId = await currentUser.GetUserIdAsync();
                if (activeUserId == Guid.Empty) return Unauthorized();

                var (savingPockets, totalCount) = await savingPocketService.GetSavingPocketsAsync(
                    activeUserId, page, pageSize, pocketId, startDate, endDate, pocketIds);

                return Ok(new
                {
                    Items= savingPockets,
                    TotalCount=totalCount,
                    PageNumber=page,
                    PageSize=pageSize,
                    TotalPages=(int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }

            [HttpPost]
            public async Task<IActionResult> CreateSavingPocket([FromBody] SavingPocketCreateDto pocket)
            {
                var activeUserId = await currentUser.GetUserIdAsync();
                if (activeUserId == Guid.Empty) return Unauthorized();

                var result = await savingPocketService.CreateSavingPocketAsync(activeUserId, pocket);
                return Ok(result);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteSavingPocket(Guid id)
            {
                var activeUserId = await currentUser.GetUserIdAsync();
                if (activeUserId == Guid.Empty) return Unauthorized();

                var wasDeleted = await savingPocketService.DeleteSavingPocketAsync(activeUserId, id);
                if (!wasDeleted)
                {
                    return NotFound("Saving pocket not found or you don't have permission to delete it.");
                }
                return NoContent();
            }
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateSavingPocket(Guid id, [FromBody] SavingPocketUpdateDto dto)
            {
                var activeUserId = await currentUser.GetUserIdAsync();
                if (activeUserId == Guid.Empty) return Unauthorized();

                var updatedSavingPocket = await savingPocketService.UpdateSavingPocketAsync(activeUserId, id, dto);
                if (updatedSavingPocket == null)
                {
                    return NotFound("Saving pocket not found or permission denied.");
                }

                return Ok(updatedSavingPocket);
            }
            [HttpGet("{id}")]
            public async Task<IActionResult> GetSavingPocketById(Guid id)
            {
                var activeUserId = await currentUser.GetUserIdAsync();
                if (activeUserId == Guid.Empty) return Unauthorized();

                var savingPocket = await savingPocketService.GetSavingPocketByIdAsync(activeUserId, id);
                if (savingPocket == null)
                {
                    return NotFound($"Saving pocket with ID {id} was not found for this user.");
                }
                return Ok(savingPocket);
            }

            [HttpPatch("{id}/status")]
            public async Task<IActionResult> UpdateSavingPocketStatus(Guid id, [FromBody] SavingPocketStatus newStatus)
            {
                var activeUserId = await currentUser.GetUserIdAsync();
                if (activeUserId == Guid.Empty) return Unauthorized();
                var wasUpdated = await savingPocketService.UpdateSavingPocketStatusAsync(activeUserId, id, newStatus);
                if (!wasUpdated)
                {
                    return NotFound("Saving pocket not found or permission denied.");
                }
                return NoContent();
            }
        }
    }
