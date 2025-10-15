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

        /* Stocks */
        public DbSet<StockPriceCache> StockPriceCache { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Force separate tables instead of TPH (Table Per Hierarchy)
            builder.Entity<Asset>().ToTable("Assets");
            builder.Entity<Profit>().ToTable("Profits");
            builder.Entity<Expense>().ToTable("Expenses");

            // Configure decimal precision for Value property (28,8 to handle both millions and crypto fractions)
            builder.Entity<Asset>().Property(p => p.Value).HasPrecision(28, 8);
            builder.Entity<Profit>().Property(p => p.Value).HasPrecision(28, 8);
            builder.Entity<Expense>().Property(p => p.Value).HasPrecision(28, 8);

            // Configure decimal precision for Interest property
            builder.Entity<Asset>().Property(p => p.Interest).HasPrecision(18, 2);

            // Configure decimal precision for Forex properties (28,8 for crypto prices)
            builder.Entity<Forex>().Property(p => p.Usd).HasPrecision(28, 8);
            builder.Entity<Forex>().Property(p => p.Eur).HasPrecision(28, 8);
            builder.Entity<Forex>().Property(p => p.Gold).HasPrecision(28, 8);
            builder.Entity<Forex>().Property(p => p.Btc).HasPrecision(28, 8);
            builder.Entity<Forex>().Property(p => p.Eth).HasPrecision(28, 8);

            // Configure StockPriceCache
            builder.Entity<StockPriceCache>().Property(p => p.Price).HasPrecision(18, 4);
            builder.Entity<StockPriceCache>().HasIndex(p => p.Ticker).IsUnique();

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
