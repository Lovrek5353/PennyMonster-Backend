using Microsoft.EntityFrameworkCore;
using PennyMonster.Enums;

namespace PennyMonster.Models
{
    public class Subscription : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime NextBillingDate { get; set; }
        [Precision(18, 2)]
        public decimal BillingAmount { get; set; }
        public BillingFrequency BillingFrequency { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public string Provider { get; set;  }  = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTime CancellationDate { get; set; }
        public Boolean AutoRenew { get; set; }

        public required Guid UserId { get; set; }

        public User? User { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
