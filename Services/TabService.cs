using Microsoft.EntityFrameworkCore;
using PennyMonster.Data;
using PennyMonster.DTOs;
using PennyMonster.Enums;
using PennyMonster.Models;

namespace PennyMonster.Services
{
    public class TabService(PennyMonsterContext context) : ITabService
    {
        public async Task<(IEnumerable<TabDto> Tabs, int TotalCount)> GetTabsAsync(Guid userId, int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate)
        {
            var query = context.Tabs
                .Where(tab => tab.UserId == userId);
            if (startDate.HasValue)
            {
                query = query.Where(tab => tab.StartDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(tab => tab.DueDate <= endDate.Value);
            }
            var totalCount = await query.CountAsync();
            var tabs = await query
                .OrderByDescending(tab => tab.CreatedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            bool madeChanges = false;
            foreach (var tab in tabs)
            {
                var oldStatus = tab.Status;
                tab.EvaluateTimeBasedStatus(DateTime.UtcNow);
                if (tab.Status != oldStatus) madeChanges = true;
            }
            if (madeChanges) await context.SaveChangesAsync();
            var tabDtos = tabs.Select(tab => new TabDto
            {
                Id = tab.Id,
                Name = tab.Name,
                Description = tab.Description,
                InitialAmount = tab.InitialAmount,
                OutstandingBalance = tab.OutstandingBalance,
                InterestRate = tab.InterestRate,
                StartDate = tab.StartDate,
                DueDate = tab.DueDate,
                BillingFrequency = tab.BillingFrequency,
                Lender = tab.Lender,
                PriorityLevel = tab.PriorityLevel,
                Color = tab.Color,
                MonthlyPayment = tab.MonthlyPayment,
                CreatedAt = tab.CreatedAtUtc,
                LastModified = tab.LastModified
            });
            return (tabDtos, totalCount);
        }
        public async Task<TabDto> CreateTabAsync(Guid userId, TabCreateDto dto)
        {
            var tab = new Tab(userId, dto.Name, dto.InitialAmount)
            {
                Description = dto.Description,
                InterestRate = dto.InterestRate,
                StartDate = dto.StartDate,
                DueDate = dto.DueDate,
                BillingFrequency = dto.BillingFrequency,
                Lender = dto.Lender,
                PriorityLevel = dto.PriorityLevel,
                Color = dto.Color,
                MonthlyPayment = dto.MonthlyPayment
            };

            context.Tabs.Add(tab);
            await context.SaveChangesAsync();

            return new TabDto
            {
                Id = tab.Id,
                Name = tab.Name,
                Description = tab.Description,
                InitialAmount = tab.InitialAmount,
                OutstandingBalance = tab.OutstandingBalance,
                InterestRate = tab.InterestRate,
                StartDate = tab.StartDate,
                DueDate = tab.DueDate,
                BillingFrequency = tab.BillingFrequency,
                Lender = tab.Lender,
                PriorityLevel = tab.PriorityLevel,
                Color = tab.Color,
                MonthlyPayment = tab.MonthlyPayment,
                CreatedAt = tab.CreatedAtUtc,
                LastModified = tab.LastModified,
            };
        }

        public async Task<bool> DeleteTabAsync(Guid userId, Guid id)
        {
            var tab=await context.Tabs
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId==userId);

            if (tab == null)
            {
                return false;
            }
            context.Tabs.Remove(tab);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<TabDto?> GetTabByIdAsync(Guid userId, Guid id)
        {
            var tab = await context.Tabs
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (tab == null) return null;

            tab.EvaluateTimeBasedStatus(DateTime.UtcNow);

            return new TabDto
            {
                Id=tab.Id,
                Name=tab.Name,
                Description=tab.Description,
                InitialAmount=tab.InitialAmount,
                OutstandingBalance=tab.OutstandingBalance,
                InterestRate=tab.InterestRate,
                StartDate=tab.StartDate,
                DueDate=tab.DueDate,
                BillingFrequency=tab.BillingFrequency,
                Lender=tab.Lender,
                PriorityLevel=tab.PriorityLevel,
                Color=tab.Color,
                MonthlyPayment=tab.MonthlyPayment,
                CreatedAt=tab.CreatedAtUtc,
                LastModified=tab.LastModified
            };
        }

        public async Task<TabDto?> UpdateTabAsync(Guid userId, Guid id, TabUpdateDto tabDto)
        {
            var tab = await context.Tabs
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (tab == null) return null;

            if (tab.InitialAmount != tabDto.InitialAmount)
            {
                tab.AdjustInitialAmount(tabDto.InitialAmount);
            }
            tab.Name=tabDto.Name;
            tab.Description=tabDto.Description;
            tab.InterestRate=tabDto.InterestRate;
            tab.StartDate=tabDto.StartDate;
            tab.DueDate=tabDto.DueDate;
            tab.BillingFrequency=tabDto.BillingFrequency;
            tab.Lender=tabDto.Lender;
            tab.PriorityLevel=tabDto.PriorityLevel;
            tab.Color=tabDto.Color;

            await context.SaveChangesAsync();

            return new TabDto
            {
                Id = tab.Id,
                Name = tab.Name,
                Description = tab.Description,
                InitialAmount = tab.InitialAmount,
                OutstandingBalance = tab.OutstandingBalance,
                InterestRate = tab.InterestRate,
                StartDate = tab.StartDate,
                DueDate = tab.DueDate,
                BillingFrequency = tab.BillingFrequency,
                Lender = tab.Lender,
                PriorityLevel = tab.PriorityLevel,
                Color = tab.Color,
                CreatedAt = tab.CreatedAtUtc,
                LastModified = tab.LastModified
            };
        }

        public async Task<bool> UpdateTabStatusAsync(Guid userId, Guid id, TabStatus newStatus)
        {
            var tab = await context.Tabs
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (tab == null) return false;

            try
            {
                switch (newStatus)
                {
                    case TabStatus.Cancelled:
                        tab.Cancel();
                        break;
                    case TabStatus.Frozen:
                        tab.Freeze();
                        break;
                    case TabStatus.Active:
                        tab.Reactivate();
                        break;
                    default:
                        throw new ArgumentException("Manual transition to this status is not allowed.");
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
