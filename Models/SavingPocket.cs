using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PennyMonster.Enums;
using System.ComponentModel.DataAnnotations;

namespace PennyMonster.Models
{
    public class SavingPocket : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Precision(18, 2)]
        public decimal CurrentBalance { get; set; } = 0.00m;
        [Precision(18, 2)]
        public decimal TargetAmount { get; private set; } = 0.00m;
        public DateTime TargetDate { get; set; }
        public string Color { get; set; } = string.Empty;
        public SavingPocketStatus SavingPocketStatus { get; private set; } = SavingPocketStatus.Active;
        public DateTime StartDate { get; set; }
        public DateTime? TargetReachedDate { get; private set; }
        [Range(0, 100)]
        public int PriorityLevel { get; set; }
        [Precision(18, 2)]
        public decimal MonthlyPayment { get; set; }
        public BillingFrequency BillingFrequency { get; set; }

        public Guid UserId { get; set; }

        public User? User { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        // --- 1. CONSTRUCTORS ---
        protected SavingPocket() { }

        public SavingPocket(Guid userId, string name, decimal targetAmount)
        {
            UserId = userId;
            Name = name;
            TargetAmount = targetAmount;
            CurrentBalance = 0.00m;
            SavingPocketStatus = SavingPocketStatus.Active;
        }

        private void EvaluateTargetStatus()
        {
            if (CurrentBalance >= TargetAmount)
            {
                if (!TargetReachedDate.HasValue) TargetReachedDate = DateTime.UtcNow;
                SavingPocketStatus = SavingPocketStatus.Completed; 
            }
            else
            {
                TargetReachedDate = null;

                if (SavingPocketStatus == SavingPocketStatus.Completed)
                {
                    EvaluateTimeBasedStatus(DateTime.UtcNow);
                    SavingPocketStatus = SavingPocketStatus.Active;
                }
            }
        }

        public void ApplyPaymnet(decimal amount)
        {
            if (SavingPocketStatus != SavingPocketStatus.Active)
                throw new InvalidOperationException("Cannot apply payment to a non-active saving pocket.");

            CurrentBalance += amount;
            EvaluateTargetStatus();
        }
        public void RevertPayment(decimal amount)
        {
            if (SavingPocketStatus != SavingPocketStatus.Active)
                throw new InvalidOperationException("Cannot revert payment on a non-active saving pocket.");
            CurrentBalance -= amount;
            EvaluateTargetStatus();
        }
        public void UpdatePayment(decimal oldAmount, decimal newAmount)
        {
            if (SavingPocketStatus != SavingPocketStatus.Active)
                throw new InvalidOperationException("Cannot update payment on a non-active saving pocket.");
            CurrentBalance = CurrentBalance - oldAmount + newAmount;
        }
        public void UpdateTargetAmount(decimal newTargetAmount)
        {
            TargetAmount = newTargetAmount;
            EvaluateTargetStatus();
        }

        // State methods
        public void Cancel()
        {
            if (SavingPocketStatus != SavingPocketStatus.Active)
                throw new InvalidOperationException("Only active saving pockets can be cancelled.");
            SavingPocketStatus = SavingPocketStatus.Cancelled;
        }

        public void Pause()
        {
            if (SavingPocketStatus != SavingPocketStatus.Active)
                throw new InvalidOperationException("Only active saving pockets can be paused.");
            SavingPocketStatus = SavingPocketStatus.Paused;
        }

        public void Resume()
        {
            if (SavingPocketStatus != SavingPocketStatus.Paused)
                throw new InvalidOperationException("Only paused saving pockets can be resumed.");
            SavingPocketStatus = SavingPocketStatus.Active;
            EvaluateTimeBasedStatus(DateTime.UtcNow);
        }

        public void EvaluateTimeBasedStatus(DateTime currentDate)
        {
            if (SavingPocketStatus == SavingPocketStatus.Active && currentDate > TargetDate)
            {
                SavingPocketStatus = SavingPocketStatus.Overdue;
            }
            else if (SavingPocketStatus == SavingPocketStatus.Overdue && currentDate <= TargetDate)
            {
                SavingPocketStatus = SavingPocketStatus.Active;
            }
        }
    }
}
