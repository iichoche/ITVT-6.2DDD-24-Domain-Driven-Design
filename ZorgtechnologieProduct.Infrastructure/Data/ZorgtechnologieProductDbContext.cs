using Microsoft.EntityFrameworkCore;
using ZorgtechnologieProduct.Domain;

namespace ZorgtechnologieProduct.Infrastructure.Data
{
    // DbContext voor de ZorgtechnologieProduct applicatie
    public class ZorgtechnologieProductDbContext : DbContext
    {
        // DbSet voor alle zorgproducten
        public DbSet<ZorgProduct> Zorgproducten { get; set; }

        // DbSet voor items die bij een zorgproduct horen (zoals gebruiksstatus)
        public DbSet<ZorgtechnologieProductItem> ZorgtechnologieProductItems { get; set; }

        // Constructor die de opties doorgeeft aan de base class
        public ZorgtechnologieProductDbContext(DbContextOptions<ZorgtechnologieProductDbContext> options)
            : base(options)
        {
        }

        // Configureert de tabellen en relaties in de database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ZorgProduct wordt opgeslagen in de tabel 'ZorgtechnologieProduct'
            modelBuilder.Entity<ZorgProduct>().ToTable("ZorgtechnologieProduct");

            // ZorgtechnologieProductItem wordt opgeslagen in de tabel 'ZorgtechnologieProductItem'
            modelBuilder.Entity<ZorgtechnologieProductItem>().ToTable("ZorgtechnologieProductItem");

            base.OnModelCreating(modelBuilder);
        }
    }
}
