using Microsoft.EntityFrameworkCore;
using ZorgtechnologieProduct.Domain;

namespace ZorgtechnologieProduct.Infrastructure.Data
{
    public class ZorgtechnologieProductDbContext : DbContext
    {
        public DbSet<ZorgProduct> Zorgproducten { get; set; }

        public ZorgtechnologieProductDbContext(DbContextOptions<ZorgtechnologieProductDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ZorgProduct>().ToTable("ZorgtechnologieProduct");
            base.OnModelCreating(modelBuilder);
        }
    }
}