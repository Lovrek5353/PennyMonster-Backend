using Microsoft.EntityFrameworkCore;
using PennyMonster.Enums;
using System.ComponentModel.DataAnnotations;

namespace PennyMonster.Models;

public class Tab : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [Precision(18, 2)]
    public decimal InitialAmount { get; private set; } = 0.00m; 

    [Precision(18, 2)]
    public decimal OutstandingBalance { get; private set; } = 0.00m;
    public TabStatus Status { get; private set; } = TabStatus.Active;

    [Precision(3, 2)]
    public decimal InterestRate { get; set; } = decimal.Zero;
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public BillingFrequency BillingFrequency { get; set; }
    public string Lender { get; set; } = string.Empty;
    public DateTime? TargetReachedDate { get; private set; } 
    [Range(0, 100)]
    public int PriorityLevel { get; set; }
    public string Color { get; set; } = string.Empty;
    public decimal MonthlyPayment { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    // --- 1. CONSTRUCTORS ---
    protected Tab() { }
    public Tab(Guid userId, string name, decimal initialAmount)
    {
        UserId = userId;
        Name = name;
        InitialAmount = initialAmount;
        OutstandingBalance = initialAmount;
        Status = TabStatus.Active;
    }

    private void EvaluateTargetStatus()
    {
        if (OutstandingBalance <= 0)
        {
            if (!TargetReachedDate.HasValue) TargetReachedDate = DateTime.UtcNow;

            Status = TabStatus.Closed;
        }
        else
        {
            TargetReachedDate = null;

            if (Status == TabStatus.Closed)
            {
                Status = TabStatus.Active;
                EvaluateTimeBasedStatus(DateTime.UtcNow);
            }
        }
    }

    public void ApplyPayment(decimal amount)
    {
        if (Status == TabStatus.Cancelled || Status == TabStatus.Frozen)
        {
            throw new InvalidOperationException($"Cannot apply payment to a {Status} tab.");
        }

        OutstandingBalance -= amount;
        EvaluateTargetStatus();
    }

    public void RevertPayment(decimal amount)
    {
        OutstandingBalance += amount;
        EvaluateTargetStatus();
    }
    public void UpdatePayment(decimal oldAmount, decimal newAmount)
    {
        RevertPayment(oldAmount);
        ApplyPayment(newAmount); 
    }

    public void AdjustInitialAmount(decimal newAmount)
    {
        decimal difference = newAmount - InitialAmount;

        OutstandingBalance += difference;

        InitialAmount = newAmount;

        EvaluateTargetStatus();
    }


    public void Cancel()
    {
        if (Status == TabStatus.Closed)
            throw new InvalidOperationException("Cannot cancel a loan that is already paid off.");

        Status = TabStatus.Cancelled;
    }

    public void Freeze()
    {
        if (Status == TabStatus.Closed || Status == TabStatus.Cancelled)
            throw new InvalidOperationException("Cannot freeze a closed or cancelled tab.");

        Status = TabStatus.Frozen;
    }

    public void Reactivate()
    {
        if (Status != TabStatus.Frozen && Status != TabStatus.Cancelled)
            throw new InvalidOperationException("Only Frozen or Cancelled tabs can be reactivated.");

        Status = TabStatus.Active;
        EvaluateTimeBasedStatus(DateTime.UtcNow);
    }

    public void EvaluateTimeBasedStatus(DateTime currentDate)
    {
        if (Status == TabStatus.Closed || Status == TabStatus.Cancelled || Status == TabStatus.Frozen)
            return;

        if (OutstandingBalance > 0)
        {
            if (currentDate.Date > DueDate.AddMonths(6).Date)
            {
                Status = TabStatus.Defaulted;
            }
            else if (currentDate.Date > DueDate.Date)
            {
                Status = TabStatus.Overdue;
            }
            else
            {
                Status = TabStatus.Active;
            }
        }
    }
}