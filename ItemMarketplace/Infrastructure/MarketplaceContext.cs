using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class MarketplaceContext : DbContext
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;


        public DbSet<Item> Item { get; set; }
        public DbSet<Auction> Auction { get; set; }


        public MarketplaceContext(IConfiguration configuration, ILogger<MarketplaceContext> logger)
        {
            _connectionString = configuration["Db:ConnectionString"] ?? throw new Exception();
            _logger = logger;

#if DEBUG
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
#endif
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString)
                .LogTo(
                    action: x => _logger.LogDebug(x), 
                    minimumLevel: LogLevel.Information, 
                    options: Microsoft.EntityFrameworkCore.Diagnostics.DbContextLoggerOptions.Level);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
                .HasOne(i => i.Item)
                .WithMany(a => a.Auctions)
                .HasForeignKey(p => p.ItemId);

            modelBuilder.Entity<Item>().HasIndex(i => new { i.Name, i.Description }).HasDatabaseName("IX_Item_Search");
        }
    }
}
