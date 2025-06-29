using Xunit;
using ZorgtechnologieProduct.Application;
using ZorgtechnologieProduct.Domain;

public class ProductServiceTests
{
    [Fact]
    public void KanProductToevoegen()
    {
        var service = new ProductService();
        var nieuwProduct = new ZorgProduct { Naam = "Test", Omschrijving = "Test", Type = "Test", Kosten = 10 };

        var resultaat = service.Create(nieuwProduct);

        Assert.NotNull(resultaat);
        Assert.Equal("Test", resultaat.Naam);
    }
}
