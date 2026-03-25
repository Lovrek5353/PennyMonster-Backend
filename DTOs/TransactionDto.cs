using PennyMonster.Enums;

namespace PennyMonster.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Note { get; set; }
        public TransactionType TransactionType { get; set; }

        // Foreign Keys
        public Guid? CategoryId { get; set; }
        public Guid? TabId { get; set; }
        public Guid? SavingPocketId { get; set; }
        public Guid? SubscriptionId { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
    public class TransactionCreateDto
    {
        public required decimal Amount { get; set; }
        public required DateTime Date { get; set; }
        public required string Description { get; set; }
        public string? Location { get; set; }
        public string? Note { get; set; }
        public TransactionType TransactionType { get; set; }

        public required Guid UserId { get; set; }

        // Optional links
        public Guid? CategoryId { get; set; }
        public Guid? TabId { get; set; }
        public Guid? SavingPocketId { get; set; }
        public Guid? SubscriptionId { get; set; }
    }

    public class TransactionUpdateDto
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Note { get; set; }
        public TransactionType TransactionType { get; set; }

        // Foreign Keys
        public Guid? CategoryId { get; set; }
        public Guid? TabId { get; set; }
        public Guid? SavingPocketId { get; set; }
        public Guid? SubscriptionId { get; set; }

    }
}
