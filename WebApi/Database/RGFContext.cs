using Microsoft.EntityFrameworkCore;
using Database.Entities;

namespace Database
{
    public class RGFContext : DbContext
    {
        public RGFContext(DbContextOptions options) : base(options)
        {
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /* Users */
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        
        /* Flows */
        public DbSet<Asset> Assets { get; set; } = null!;        
        public DbSet<Profit> Profits { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;

        /* Forex */
        public DbSet<Forex> Forexes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Force separate tables instead of TPH (Table Per Hierarchy)
            builder.Entity<Asset>().ToTable("Assets");
            builder.Entity<Profit>().ToTable("Profits"); 
            builder.Entity<Expense>().ToTable("Expenses");

            // Configure Currency enum conversion for each entity
            builder
                .Entity<Asset>()
                .Property(c => c.Currency)
                .HasConversion(
                    s => s.ToString(),
                    s => (CurrencyType)Enum.Parse(typeof(CurrencyType), s));

            builder
                .Entity<Profit>()
                .Property(c => c.Currency)
                .HasConversion(
                    s => s.ToString(),
                    s => (CurrencyType)Enum.Parse(typeof(CurrencyType), s));

            builder
                .Entity<Expense>()
                .Property(c => c.Currency)
                .HasConversion(
                    s => s.ToString(),
                    s => (CurrencyType)Enum.Parse(typeof(CurrencyType), s));

            builder
                .Entity<Asset>()
                .Property(c => c.InterestRate)
                .HasConversion(
                    s => s.ToString(),
                    s => (Rate)Enum.Parse(typeof(Rate), s));

            builder
                .Entity<Profit>()
                .Property(c => c.Rate)
                .HasConversion(
                    s => s.ToString(),
                    s => (Rate)Enum.Parse(typeof(Rate), s));

            builder
                .Entity<Expense>()
                .Property(c => c.Rate)
                .HasConversion(
                    s => s.ToString(),
                    s => (Rate)Enum.Parse(typeof(Rate), s));
        }
    }
}
