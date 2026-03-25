using Microsoft.AspNetCore.ResponseCompression;

namespace PennyMonster.Models
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Color { get; set; }

        public required Guid UserId { get; set; }

        public User? User { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
