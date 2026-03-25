using PennyMonster.DTOs;

namespace PennyMonster.Services;

public interface ITransactionService
{
    Task<(IEnumerable<TransactionDto> Items, int TotalCount)> GetTransactionsAsync(
            Guid userId,
            int page = 1,
            int pageSize = 20,
            Guid? tabId = null,
            Guid? categoryId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? location = null,
            List<Guid>? transactionIds = null 
        );
    Task<TransactionDto> CreateTransactionAsync(Guid userId, TransactionCreateDto dto);

    Task<bool> DeleteTransactionAsync(Guid userId, Guid transactionId);
    Task <TransactionDto?> UpdateTransactionAsync(Guid userId, Guid transactionId, TransactionUpdateDto transactionDto);
    Task <TransactionDto?> GetTransactionByIdAsync(Guid userId, Guid transactionId);
}