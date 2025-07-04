using Xunit;
using Microsoft.EntityFrameworkCore;
using ZorgtechnologieProduct.Infrastructure.Data;
using ZorgtechnologieProduct.Domain;
using System;

// Testklasse voor integratietests
public class IntegrationTests
{
    // Test: een product wordt opgeslagen en kan weer uit de database gehaald worden
    [Fact]
    public void ProductWordtOpgeslagenInDatabase()
    {
        // Maak een in-memory database aan voor de test
        var options = new DbContextOptionsBuilder<ZorgtechnologieProductDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Voeg een product toe aan de database
        using var context = new ZorgtechnologieProductDbContext(options);
        var product = new ZorgProduct { Id = Guid.NewGuid(), Naam = "Demo", Omschrijving = "Demo", Type = "Demo", Kosten = 1 };
        context.Zorgproducten.Add(product);
        context.SaveChanges();

        // Haal het product weer uit de database en controleer of het klopt
        var uitDb = context.Zorgproducten.Find(product.Id);
        Assert.NotNull(uitDb);
        Assert.Equal("Demo", uitDb.Naam);
    }
}