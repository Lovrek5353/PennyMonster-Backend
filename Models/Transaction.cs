using Microsoft.EntityFrameworkCore;
using PennyMonster.Enums;

namespace PennyMonster.Models
{
    public class Transaction : BaseEntity
    {
        [Precision(18, 2)]
        public required decimal Amount { get; set; }
        public required DateTime Date { get; set; }
        public required string Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }


        public required Guid UserId { get; set; }

        public User? User { get; set; }

        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        public Guid? TabId { get; set; }
        public Tab? Tab { get; set; }

        public Guid? SavingPocketId { get; set; }
        public SavingPocket? SavingPocket { get; set; }

        public Guid? SubscriptionId {  get; set; }
        public Subscription? Subscription { get; set; }

    }
}
