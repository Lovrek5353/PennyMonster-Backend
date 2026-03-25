using Microsoft.EntityFrameworkCore;
using PennyMonster.Models;

namespace PennyMonster.Data;

public class PennyMonsterContext(DbContextOptions<PennyMonsterContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Tab> Tabs => Set<Tab>();
    public DbSet<SavingPocket> SavingPockets => Set<SavingPocket>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Transaction>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<Tab>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<SavingPocket>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<Subscription>().HasQueryFilter(t => !t.IsDeleted);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.User)
            .WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
        .HasIndex(u => u.FirebaseUid)
        .IsUnique();


        modelBuilder.Entity<Tab>().Property(t => t.InitialAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Tab>().Property(t => t.OutstandingBalance).HasPrecision(18, 2);
        modelBuilder.Entity<Tab>().Property(t => t.MonthlyPayment).HasPrecision(18, 2);

        modelBuilder.Entity<SavingPocket>().Property(p => p.TargetAmount).HasPrecision(18, 2);
        modelBuilder.Entity<SavingPocket>().Property(p => p.MonthlyPayment).HasPrecision(18, 2);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                    entry.Entity.LastModified = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModified = DateTime.UtcNow;
                    entry.Entity.Version++;
                    break;

                case EntityState.Deleted:
                    // --- HARD DELETE LOGIC ---
                    if (entry.Entity.IsDeleted)
                    {
                        break;
                    }

                    // --- SOFT DELETE LOGIC ---
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.LastModified = DateTime.UtcNow;
                    entry.Entity.Version++;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}