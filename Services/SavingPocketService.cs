using Microsoft.EntityFrameworkCore;
using PennyMonster.Data;
using PennyMonster.DTOs;
using PennyMonster.Enums;
using PennyMonster.Models;

namespace PennyMonster.Services
{
    public class SavingPocketService(PennyMonsterContext context) : ISavingPocketService
    {
        public async Task<(IEnumerable<SavingPocketDto> Items, int TotalCount)> GetSavingPocketsAsync(
            Guid userId,
            int page = 1,
            int pageSize = 20,
            Guid? pocketId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            List<Guid>? pocketIds = null)
        {
            var query = context.SavingPockets.Where(p => p.UserId == userId);

            if (pocketId.HasValue)
                query = query.Where(p => p.Id == pocketId.Value);

            if (startDate.HasValue)
                query = query.Where(p => p.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.StartDate <= endDate.Value);

            if (pocketIds != null && pocketIds.Any())
                query = query.Where(p => pocketIds.Contains(p.Id));

            int totalCount = await query.CountAsync();

            var pockets = await query
                .OrderByDescending(p => p.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            bool madeChanges = false;
            var now = DateTime.UtcNow;

            foreach (var pocket in pockets)
            {
                var oldStatus = pocket.SavingPocketStatus;
                pocket.EvaluateTimeBasedStatus(now);

                if (pocket.SavingPocketStatus != oldStatus)
                    madeChanges = true;
            }

            if (madeChanges) await context.SaveChangesAsync();

            var mappedItems = pockets.Select(pocket => new SavingPocketDto
            {
                Id = pocket.Id,
                Name = pocket.Name,
                Description = pocket.Description,
                TargetAmount = pocket.TargetAmount,
                TargetDate = pocket.TargetDate,
                SavingPocketStatus = pocket.SavingPocketStatus,
                StartDate = pocket.StartDate,
                PriorityLevel = pocket.PriorityLevel,
                Color = pocket.Color,
                MonthlyPayment = pocket.MonthlyPayment,
                CreatedAt = pocket.CreatedAtUtc,
                LastModified = pocket.LastModified
            });

            return (mappedItems, totalCount);
        }

        public async Task<SavingPocketDto> CreateSavingPocketAsync(Guid userId, SavingPocketCreateDto dto)
        {
            var pocket = new SavingPocket(userId, dto.Name, dto.TargetAmount)
            {
                Name = dto.Name,
                Description = dto.Description,
                TargetDate = dto.TargetDate,
                StartDate = dto.StartDate,
                PriorityLevel = dto.PriorityLevel,
                BillingFrequency = dto.BillingFrequency,
                Color = dto.Color,
            };

            context.SavingPockets.Add(pocket);
            await context.SaveChangesAsync();

            return new SavingPocketDto
            {
                Id = pocket.Id,
                Name = pocket.Name,
                Description=pocket.Description,
                TargetAmount= pocket.TargetAmount,
                TargetDate= pocket.TargetDate,
                SavingPocketStatus= pocket.SavingPocketStatus,
                StartDate= pocket.StartDate,
                PriorityLevel= pocket.PriorityLevel,
                Color=pocket.Color,
                MonthlyPayment= pocket.MonthlyPayment,
                CreatedAt=pocket.CreatedAtUtc,
                LastModified=pocket.LastModified
            };
        }

        public async Task<bool> DeleteSavingPocketAsync(Guid userId, Guid pocketId)
        {
            var pocket = await context.SavingPockets
                .FirstOrDefaultAsync(p => p.Id == pocketId && p.UserId == userId);

            if (pocket == null)
            {
                return false;
            }
            context.SavingPockets.Remove(pocket);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<SavingPocketDto?> GetSavingPocketByIdAsync(Guid userId, Guid pocketId)
        {
            var pocket = await context.SavingPockets
                .FirstOrDefaultAsync(p => p.Id == pocketId && p.UserId == userId);

            if (pocket == null) return null;

            return new SavingPocketDto
            {
                Id = pocket.Id,
                Name = pocket.Name,
                Description = pocket.Description,
                TargetAmount = pocket.TargetAmount,
                TargetDate = pocket.TargetDate,
                SavingPocketStatus = pocket.SavingPocketStatus,
                StartDate = pocket.StartDate,
                PriorityLevel = pocket.PriorityLevel,
                Color = pocket.Color,
                MonthlyPayment = pocket.MonthlyPayment,
                CreatedAt = pocket.CreatedAtUtc,
                LastModified = pocket.LastModified
            };
        }

        public async Task<SavingPocketDto?> UpdateSavingPocketAsync(Guid userId, Guid id, SavingPocketUpdateDto dto)
        {
            var pocket = await context.SavingPockets
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (pocket == null) return null;

            if(pocket.TargetAmount != dto.TargetAmount)
            {
                pocket.UpdateTargetAmount(dto.TargetAmount);
            }

            pocket.Name=dto.Name;
            pocket.Description= dto.Description;
            pocket.TargetDate= dto.TargetDate;
            pocket.StartDate= dto.StartDate;
            pocket.PriorityLevel= dto.PriorityLevel;
            pocket.BillingFrequency= dto.BillingFrequency;
            pocket.Color= dto.Color;
            pocket.MonthlyPayment= dto.MonthlyPayment;

            await context.SaveChangesAsync();


            return new SavingPocketDto
            {
                Id = pocket.Id,
                Name = pocket.Name,
                Description = pocket.Description,
                TargetAmount = pocket.TargetAmount,
                TargetDate = pocket.TargetDate,
                SavingPocketStatus = pocket.SavingPocketStatus,
                StartDate = pocket.StartDate,
                PriorityLevel = pocket.PriorityLevel,
                Color = pocket.Color,
                MonthlyPayment = pocket.MonthlyPayment,
                CreatedAt = pocket.CreatedAtUtc,
                LastModified = pocket.LastModified
            };
        }

        public async Task<bool> UpdateSavingPocketStatusAsync(Guid userId, Guid id, SavingPocketStatus newStatus)
        {
            var pocket = await context.SavingPockets
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (pocket == null)
            {
                return false;
            }

            try
            {
                switch (newStatus)
                {
                    case SavingPocketStatus.Active:
                        pocket.Resume();
                        break;
                    case SavingPocketStatus.Paused:
                        pocket.Pause();
                        break;
                    case SavingPocketStatus.Cancelled:
                        pocket.Cancel();
                        break;
                    default:
                        throw new InvalidOperationException("Invalid status transition");
                }
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}