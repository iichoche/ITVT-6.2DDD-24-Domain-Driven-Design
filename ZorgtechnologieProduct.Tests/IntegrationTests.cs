using Xunit;
using Microsoft.EntityFrameworkCore;
using ZorgtechnologieProduct.Infrastructure.Data;
using ZorgtechnologieProduct.Domain;
using System;

public class IntegrationTests
{
    [Fact]
    public void ProductWordtOpgeslagenInDatabase()
    {
        var options = new DbContextOptionsBuilder<ZorgtechnologieProductDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new ZorgtechnologieProductDbContext(options);
        var product = new ZorgProduct { Id = Guid.NewGuid(), Naam = "Demo", Omschrijving = "Demo", Type = "Demo", Kosten = 1 };
        context.Zorgproducten.Add(product);
        context.SaveChanges();

        var uitDb = context.Zorgproducten.Find(product.Id);
        Assert.NotNull(uitDb);
        Assert.Equal("Demo", uitDb.Naam);
    }
}