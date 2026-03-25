namespace PennyMonster.Models;

public class User : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }

    public bool IsEmailVerified { get; set; } = false;

    public required string FirebaseUid { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public ICollection<Tab> Tabs { get; set; } = new List<Tab>();
    public ICollection<SavingPocket> SavingPockets { get; set; }= new List<SavingPocket>();
    public ICollection<Subscription> Subscriptions { get; set; }= new List<Subscription>();

}