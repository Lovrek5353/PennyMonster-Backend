using Microsoft.EntityFrameworkCore;
using PennyMonster.Enums;
using PennyMonster.Models;
using System.ComponentModel.DataAnnotations;

namespace PennyMonster.DTOs
{
    public class SavingPocketDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public DateTime TargetDate { get; set; }
        public SavingPocketStatus SavingPocketStatus {  get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetReachedDate { get; set; }
        public int PriorityLevel {  get; set; }
        public BillingFrequency BillingFrequency { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal MonthlyPayment {  get; set; }

        // Audit 
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
    public class SavingPocketCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime StartDate { get; set; }
        public int PriorityLevel { get; set; }
        public BillingFrequency BillingFrequency { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal MonthlyPayment { get; set; }
        public required Guid UserId { get; set; }

    }

    public class SavingPocketUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime StartDate { get; set; }
        public int PriorityLevel { get; set; }
        public BillingFrequency BillingFrequency { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal MonthlyPayment { get; set; }
    }
}
