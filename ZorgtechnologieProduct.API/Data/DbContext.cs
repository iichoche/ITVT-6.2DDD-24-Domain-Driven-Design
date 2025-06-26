using Microsoft.EntityFrameworkCore;
using ZorgtechnologieProduct.Domain;

namespace ZorgtechnologieProduct.API.Data
{
    public class ZorgtechnologieDbContext : DbContext
    {
        public ZorgtechnologieDbContext(DbContextOptions<ZorgtechnologieDbContext> options) : base(options)
        {
        }

        public DbSet<ZorgtechnologieProduct.Domain.ZorgtechnologieProduct> ZorgtechnologieProducten { get; set; }

        public DbSet<ZorgtechnologieProductItem> ZorgtechnologieProductItems { get; set; }
        public DbSet<Zorginstellingslocatie> Zorginstellingslocaties { get; set; }
    }
}
