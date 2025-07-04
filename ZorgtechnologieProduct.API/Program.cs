using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ZorgtechnologieProduct.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System;
using ZorgtechnologieProduct.Domain;
using System.Linq;

namespace ZorgtechnologieProduct.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Bouw de webapplicatie
            var builder = WebApplication.CreateBuilder(args);

            // Haal de omgeving op (Development, Production, Testing, etc.)
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var useInMemory = false;

            // Probeer verbinding te maken met SQL Server, anders gebruik InMemory database
            if (environment == "Testing")
            {
                // Gebruik altijd InMemory in testomgeving
                builder.Services.AddDbContext<ZorgtechnologieProductDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
                useInMemory = true;
            }
            else
            {
                // Probeer verbinding te testen met SQL Server
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ZorgtechnologieProductDbContext>();
                    optionsBuilder.UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly("ZorgtechnologieProduct.Infrastructure")
                    );
                    // Open en sluit de verbinding om te testen of de database bereikbaar is
                    using (var testDb = new ZorgtechnologieProductDbContext(optionsBuilder.Options))
                    {
                        testDb.Database.OpenConnection();
                        testDb.Database.CloseConnection();
                    }
                    // Als het lukt, registreer SQL Server als database
                    builder.Services.AddDbContext<ZorgtechnologieProductDbContext>(options =>
                        options.UseSqlServer(
                            builder.Configuration.GetConnectionString("DefaultConnection"),
                            b => b.MigrationsAssembly("ZorgtechnologieProduct.Infrastructure")
                        )
                    );
                }
                catch
                {
                    // Als het niet lukt, gebruik een InMemory database als fallback
                    builder.Services.AddDbContext<ZorgtechnologieProductDbContext>(options =>
                        options.UseInMemoryDatabase("FallbackDb"));
                    useInMemory = true;
                }
            }

            // Voeg controllers, CORS, Swagger en logging toe aan de services
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();

            // Bouw de app
            var app = builder.Build();

            // Voeg dummy data toe als de InMemory database wordt gebruikt
            if (useInMemory)
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ZorgtechnologieProductDbContext>();
                    db.Database.EnsureCreated();
                    if (!db.Zorgproducten.Any())
                    {
                        db.Zorgproducten.AddRange(
                            new ZorgProduct
                            {
                                Id = Guid.NewGuid(),
                                Naam = "Demo Product 1",
                                Omschrijving = "Dummy product voor demo",
                                Type = "Sensor",
                                Kosten = 100
                            },
                            new ZorgProduct
                            {
                                Id = Guid.NewGuid(),
                                Naam = "Demo Product 2",
                                Omschrijving = "Nog een dummy product",
                                Type = "Monitor",
                                Kosten = 200
                            }
                        );
                        db.SaveChanges();
                    }
                }
            }

            // Configureer middleware voor Swagger, CORS, autorisatie en controllers
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors();
            app.UseAuthorization();
            app.MapControllers();

            // Log de gebruikte connection string en of InMemory wordt gebruikt
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"[DEBUG] Connection string: {connectionString}");
            if (useInMemory)
                Console.WriteLine("[INFO] Using in-memory database with dummy data.");

            // Start de applicatie
            app.Run();
        }
    }
}