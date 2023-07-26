using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public class MarketplaceContext : DbContext
    {
        private readonly string _connectionString;


        public DbSet<Item> Item { get; set; }
        public DbSet<Auction> Auction { get; set; }


        public MarketplaceContext(IConfiguration configuration)
        {
            _connectionString = configuration["Db:ConnectionString"] ?? throw new Exception();


#if DEBUG
            //Database.EnsureDeleted();
#endif
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
                .HasOne(i => i.Item)
                .WithMany(a => a.Auctions)
                .HasForeignKey(p => p.ItemId);
        }
    }
}
