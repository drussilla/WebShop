using Microsoft.EntityFrameworkCore;

namespace WebShop.Data
{
    public class WebShopContext : DbContext
    {
        public WebShopContext(DbContextOptions<WebShopContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // create index on Key property since we will use it to identify the product
            modelBuilder.Entity<Product>()
                .HasIndex(x => x.Key)
                .IsUnique();
        }
    }
}