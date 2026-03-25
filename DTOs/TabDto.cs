using Microsoft.EntityFrameworkCore;
using PennyMonster.Enums;
using PennyMonster.Models;
using System.ComponentModel.DataAnnotations;

namespace PennyMonster.DTOs
{
    public class TabDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal InitialAmount { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public BillingFrequency BillingFrequency { get; set; }
        public TabStatus TabStatus { get; set; }
        public string Lender { get; set; } = string.Empty;
        public DateTime TargetReachedDate { get; set; }
        public int PriorityLevel { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal MonthlyPayment { get; set; }
        //Audit
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class TabCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal InitialAmount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public BillingFrequency BillingFrequency { get; set; }
        public string Lender { get; set; } = string.Empty;
        public int PriorityLevel { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal MonthlyPayment { get; set; }
        public required Guid UserId { get; set; }

    }

    public class TabUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty ;
        public decimal InitialAmount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public BillingFrequency BillingFrequency { get; set; }
        public string Lender { get; set; } = string.Empty;
        public int PriorityLevel { get; set; }
        public string Color { get; set; } = string.Empty;
        public decimal MonthlyPayment { get; set; }
    }
}
