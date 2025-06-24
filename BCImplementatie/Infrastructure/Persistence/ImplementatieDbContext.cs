
using Microsoft.EntityFrameworkCore;
using BCImplementatie.Domain.Entities;
using BCImplementatie.Domain.ValueObjects;

namespace BCImplementatie.Infrastructure.Persistence ;
public class ImplementatieDbContext : DbContext
{
    public ImplementatieDbContext(DbContextOptions<ImplementatieDbContext> options)
        : base(options)
    {
    }

    public DbSet<Gebruik> Gebruiken { get; set; }
    public DbSet<CareNeed> CareNeeds { get; set; }
    public DbSet<Ervaring> Ervaringen { get; set; }
    public DbSet<NeedCategory> NeedCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Gebruik
        modelBuilder.Entity<Gebruik>(b =>
        {
            b.HasKey(g => g.Id);

            // 1–N naar CareNeed
            b.HasMany(g => g.CareNeeds)
             .WithOne(c => c.Gebruik)
             .HasForeignKey(c => c.GebruikId)
             .OnDelete(DeleteBehavior.Cascade);

            // 1–N naar Ervaring
            b.HasMany(g => g.Ervaringen)
             .WithOne(e => e.Gebruik)
             .HasForeignKey(e => e.GebruikId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // 2. CareNeed
        modelBuilder.Entity<CareNeed>(b =>
        {
            b.HasKey(c => c.Id);

            b.Property(c => c.NeedDescription)
             .HasMaxLength(500)
             .IsRequired();

            // FK → lookup-tabel NeedCategory
            b.HasOne(c => c.Category)
             .WithMany()
             .HasForeignKey(c => c.NeedCategoryName)
             .IsRequired();
        });

        // 3. Ervaring
        modelBuilder.Entity<Ervaring>(b =>
        {
            b.HasKey(e => e.Id);

            b.Property(e => e.Datum).IsRequired();
            b.Property(e => e.Review).HasMaxLength(500);
            b.Property(e => e.Observatie).HasMaxLength(500);

            // FK → parent Gebruik
            b.HasOne(e => e.Gebruik)
             .WithMany(g => g.Ervaringen)
             .HasForeignKey(e => e.GebruikId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // 4. NeedCategory
        modelBuilder.Entity<NeedCategory>(b =>
        {
            b.HasKey(n => n.Name);

            b.Property(n => n.Description)
             .HasMaxLength(255)
             .IsRequired();
            //Nog aanpassen op data van Lucas
            b.HasData(
                new NeedCategory("Gezondheid", "Gezondheidszorg gerelateerde behoeften"),
                new NeedCategory("Onderwijs", "Onderwijs gerelateerde behoeften")
            );
        });
    }
}
