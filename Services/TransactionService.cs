using Microsoft.EntityFrameworkCore;
using PennyMonster.Data;
using PennyMonster.DTOs;
using PennyMonster.Enums;
using PennyMonster.Models;

namespace PennyMonster.Services;

public class TransactionService(PennyMonsterContext context) : ITransactionService
{
    public async Task<(IEnumerable<TransactionDto> Items, int TotalCount)> GetTransactionsAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        Guid? tabId = null,
        Guid? categoryId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? location = null,
        List<Guid>? transactionIds = null)
    {
        var query = context.Transactions.Where(t => t.UserId == userId);

        if (tabId.HasValue)
            query = query.Where(t => t.TabId == tabId.Value);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(t => t.Location.Contains(location));

        if (transactionIds != null && transactionIds.Any())
            query = query.Where(t => transactionIds.Contains(t.Id));

        int totalCount = await query.CountAsync();

        var transactions = await query
            .OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedItems = transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Date = t.Date,
            Description = t.Description,
            Location = t.Location,
            Note = t.Note,
            TransactionType = t.TransactionType,
            CategoryId = t.CategoryId,
            TabId = t.TabId,
            SavingPocketId = t.SavingPocketId,
            SubscriptionId = t.SubscriptionId,
            CreatedAt = t.CreatedAtUtc,
            LastModified = t.LastModified
        });

        return (mappedItems, totalCount);
    }

    public async Task<TransactionDto> CreateTransactionAsync(Guid userId, TransactionCreateDto dto)
    {
        var transaction = new Transaction
        {
            Amount = dto.Amount,
            Date = dto.Date,
            Description = dto.Description,
            Location = dto.Location,
            Note = dto.Note,
            TransactionType = dto.TransactionType,
            UserId =userId,
            CategoryId = dto.CategoryId,
            TabId = dto.TabId,
            SavingPocketId = dto.SavingPocketId,
            SubscriptionId = dto.SubscriptionId
        };

        if (dto.TabId.HasValue)
        {
            var tab = await context.Tabs
                .FirstOrDefaultAsync(t => t.Id == dto.TabId.Value && t.UserId == userId);

            if (tab != null)
            {
                tab.ApplyPayment(dto.Amount);
            }
        }

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        return new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Date = transaction.Date,
            Description = transaction.Description,
            Location = transaction.Location,
            Note = transaction.Note,
            TransactionType = transaction.TransactionType,
            CategoryId = transaction.CategoryId,
            TabId = transaction.TabId,
            SavingPocketId = transaction.SavingPocketId,
            SubscriptionId = transaction.SubscriptionId,
            CreatedAt = transaction.CreatedAtUtc,
            LastModified = transaction.LastModified
        };
    }

    public async Task<bool> DeleteTransactionAsync(Guid userId, Guid transactionId)
    {
        var transaction= await context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

        if(transaction == null)
        {
            return false;
        }

        if (transaction.TabId.HasValue)
        {
            var tab = await context.Tabs.FindAsync(transaction.TabId.Value);
            if (tab != null)
            {
                tab.RevertPayment(transaction.Amount);
            }
        }

        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<TransactionDto?> UpdateTransactionAsync(Guid userId, Guid transactionId, TransactionUpdateDto transactionDto)
    {
        var transaction = await context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

        if (transaction == null) return null;

        bool tabChanged = transaction.TabId != transactionDto.TabId;

        if (tabChanged)
        {
            if (transaction.TabId.HasValue)
            {
                var oldTab = await context.Tabs.FindAsync(transaction.TabId.Value);
                oldTab?.RevertPayment(transaction.Amount);
            }

            if (transactionDto.TabId.HasValue)
            {
                var newTab = await context.Tabs.FindAsync(transactionDto.TabId.Value);
                newTab?.ApplyPayment(transactionDto.Amount);
            }
        }
        else
        {
            if (transaction.TabId.HasValue)
            {
                var tab = await context.Tabs.FindAsync(transaction.TabId.Value);
                tab?.UpdatePayment(transaction.Amount, transactionDto.Amount);
            }
        }

        transaction.Amount = transactionDto.Amount;
        transaction.Date = transactionDto.Date;
        transaction.Description=transactionDto.Description;
        transaction.Location=transactionDto.Location;
        transaction.Note=transactionDto.Note;
        transaction.TransactionType=transactionDto.TransactionType;
        transaction.CategoryId=transactionDto.CategoryId;
        transaction.TabId=transactionDto.TabId;
        transaction.SavingPocketId=transactionDto.SavingPocketId;
        transaction.SubscriptionId=transactionDto.SubscriptionId;

        await context.SaveChangesAsync();

        return new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Location = transaction.Location,
            Note = transaction.Note,
            TransactionType = transaction.TransactionType,
            CategoryId = transaction.CategoryId,
            TabId = transaction.TabId,
            SavingPocketId = transaction.SavingPocketId,
            SubscriptionId = transaction.SubscriptionId,
            LastModified = transaction.LastModified,
        };

    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(Guid userId, Guid transactionId)
    {
        var transaction = await context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

        if (transaction == null) return null;

        return new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Location = transaction.Location,
            Note = transaction.Note,
            TransactionType = transaction.TransactionType,
            CategoryId = transaction.CategoryId,
            TabId = transaction.TabId,
            SavingPocketId = transaction.SavingPocketId,
            SubscriptionId = transaction.SubscriptionId,
            LastModified = transaction.LastModified,
        };
    }
}