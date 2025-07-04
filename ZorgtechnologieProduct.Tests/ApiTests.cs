using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ZorgtechnologieProduct.Infrastructure.Data;
using ZorgtechnologieProduct.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;
using System.Collections.Generic;

// Custom WebApplicationFactory om de testomgeving te configureren
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Zet de environment variable voor de test host op 'Testing'
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        builder.UseEnvironment("Testing");
    }
}

// Testklasse voor de API
public class ApiTests : IClassFixture<CustomWebApplicationFactory<ZorgtechnologieProduct.API.Program>>
{
    private readonly HttpClient _client;

    // Constructor: seed testdata en maak een HttpClient aan
    public ApiTests(CustomWebApplicationFactory<ZorgtechnologieProduct.API.Program> factory)
    {
        // Voeg testdata toe aan de in-memory database
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ZorgtechnologieProductDbContext>();
            db.Database.EnsureCreated();
            if (!db.Zorgproducten.Any(p => p.Naam == "TestProduct"))
            {
                db.Zorgproducten.Add(new ZorgProduct
                {
                    Id = Guid.NewGuid(),
                    Naam = "TestProduct",
                    Omschrijving = "TestOmschrijving",
                    Type = "TestType",
                    Kosten = 1
                });
                db.SaveChanges();
            }
        }

        _client = factory.CreateClient();
    }

    // Test: GET /api/producten geeft een succesvolle response
    [Fact]
    public async Task GetProducten_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/producten");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("naam", content.ToLower());
    }

    // Test: GET /api/producten bevat het testproduct
    [Fact]
    public async Task GetProducten_ResponseContainsTestProduct()
    {
        var response = await _client.GetAsync("/api/producten");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("TestProduct", content);
    }

    // Test: GET /api/producten geeft JSON terug
    [Fact]
    public async Task GetProducten_ResponseIsJson()
    {
        var response = await _client.GetAsync("/api/producten");
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
    }

    // Test: POST /api/producten voegt een product toe en geeft Created terug
    [Fact]
    public async Task PostProducten_ReturnsCreatedAndContainsProduct()
    {
        var nieuwProduct = new
        {
            naam = "PostTest",
            omschrijving = "Omschrijving via POST",
            type = "TypeX",
            kosten = 5
        };

        var response = await _client.PostAsJsonAsync("/api/producten", nieuwProduct);

        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("PostTest", content);
    }

    // Test: PUT /api/producten/{id} werkt en past het product aan
    [Fact]
    public async Task PutProduct_ReturnsNoContent_AndUpdatesProduct()
    {
        // Haal eerst een bestaand product op
        var getResponse = await _client.GetAsync("/api/producten");
        getResponse.EnsureSuccessStatusCode();
        var producten = await getResponse.Content.ReadFromJsonAsync<List<ZorgProduct>>();
        var productToUpdate = producten.First();

        var updatedProduct = new
        {
            id = productToUpdate.Id,
            naam = "UpdatedNaam",
            omschrijving = "UpdatedOmschrijving",
            type = "UpdatedType",
            kosten = 10
        };

        var putResponse = await _client.PutAsJsonAsync($"/api/producten/{productToUpdate.Id}", updatedProduct);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, putResponse.StatusCode);

        // Controleer of de update is doorgevoerd
        var getUpdatedResponse = await _client.GetAsync($"/api/producten/{productToUpdate.Id}");
        getUpdatedResponse.EnsureSuccessStatusCode();
        var updatedContent = await getUpdatedResponse.Content.ReadFromJsonAsync<ZorgProduct>();
        Assert.Equal("UpdatedNaam", updatedContent.Naam);
        Assert.Equal("UpdatedOmschrijving", updatedContent.Omschrijving);
        Assert.Equal("UpdatedType", updatedContent.Type);
        Assert.Equal(10, updatedContent.Kosten);
    }

    // Test: DELETE /api/producten/{id} verwijdert een product
    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_AndProductIsDeleted()
    {
        // Voeg eerst een product toe dat verwijderd gaat worden
        var nieuwProduct = new
        {
            naam = "DeleteTest",
            omschrijving = "Omschrijving delete test",
            type = "TypeDelete",
            kosten = 7
        };
        var postResponse = await _client.PostAsJsonAsync("/api/producten", nieuwProduct);
        postResponse.EnsureSuccessStatusCode();
        var createdProduct = await postResponse.Content.ReadFromJsonAsync<ZorgProduct>();

        // Stuur een DELETE request
        var deleteResponse = await _client.DeleteAsync($"/api/producten/{createdProduct.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Controleer dat het product niet meer bestaat
        var getResponse = await _client.GetAsync($"/api/producten/{createdProduct.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
