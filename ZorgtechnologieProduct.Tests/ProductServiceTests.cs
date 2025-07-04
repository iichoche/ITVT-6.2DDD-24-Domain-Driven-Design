using Xunit;
using ZorgtechnologieProduct.Application;
using ZorgtechnologieProduct.Domain;

// Testklasse voor de ProductService
public class ProductServiceTests
{
    // Test: een product kan worden toegevoegd aan de service
    [Fact]
    public void KanProductToevoegen()
    {
        var service = new ProductService(); // Maak een nieuwe service aan
        var nieuwProduct = new ZorgProduct { Naam = "Test", Omschrijving = "Test", Type = "Test", Kosten = 10 };

        var resultaat = service.Create(nieuwProduct); // Voeg het product toe

        Assert.NotNull(resultaat); // Controleer dat het resultaat niet null is
        Assert.Equal("Test", resultaat.Naam); // Controleer dat de naam klopt
    }
}
