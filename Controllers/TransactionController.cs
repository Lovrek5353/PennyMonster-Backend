using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyMonster.DTOs;
using PennyMonster.Services;

namespace PennyMonster.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController(ITransactionService transactionService, IUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? tabId = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? location = null,
        [FromQuery] List<Guid>? transactionIds = null)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var activeUserId = await currentUser.GetUserIdAsync();
        if (activeUserId == Guid.Empty) return Unauthorized();

        var (items, totalCount) = await transactionService.GetTransactionsAsync(
            activeUserId, page, pageSize, tabId, categoryId, startDate, endDate, location, transactionIds);

        return Ok(new
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] TransactionCreateDto dto,
        [FromServices] IValidator<TransactionCreateDto> validator)
    {

        var activeUserId = await currentUser.GetUserIdAsync();
        if (activeUserId == Guid.Empty) return Unauthorized();

        dto.UserId = activeUserId;

        var validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(errorMessages);
        }

        var result = await transactionService.CreateTransactionAsync(activeUserId, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        var activeUserId = await currentUser.GetUserIdAsync();
        if (activeUserId == Guid.Empty) return Unauthorized();

        var wasDeleted = await transactionService.DeleteTransactionAsync(activeUserId, id);
        if (!wasDeleted)
        {
            return NotFound("Transaction not found or you don't have permission to delete it.");
        }

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody]TransactionUpdateDto dto)
    {
        var activeUserId = await currentUser.GetUserIdAsync();
        if (activeUserId == Guid.Empty) return Unauthorized();

        var updatedTransaction=await transactionService.UpdateTransactionAsync(activeUserId,id, dto);

        if(updatedTransaction == null)
        {
            return NotFound("Transaction not found or you do not have permission to edit it.");
        }

        return Ok(updatedTransaction);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        var activeUserId = await currentUser.GetUserIdAsync();
        if (activeUserId == Guid.Empty) return Unauthorized();

        var transaction=await transactionService.GetTransactionByIdAsync(activeUserId, id);

        if(transaction == null)
        {
            return NotFound($"Transaction with ID {id} was not found for this user.");
        }

        return Ok(transaction);
    }
}