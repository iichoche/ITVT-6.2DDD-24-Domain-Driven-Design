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

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Force the environment variable for the test host
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        builder.UseEnvironment("Testing");
    }
}

public class ApiTests : IClassFixture<CustomWebApplicationFactory<ZorgtechnologieProduct.API.Program>>
{
    private readonly HttpClient _client;

    public ApiTests(CustomWebApplicationFactory<ZorgtechnologieProduct.API.Program> factory)
    {
        // Seed testdata
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

    [Fact]
    public async Task GetProducten_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/producten");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("naam", content.ToLower());
    }

    [Fact]
    public async Task GetProducten_ResponseContainsTestProduct()
    {
        var response = await _client.GetAsync("/api/producten");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("TestProduct", content);
    }

    [Fact]
    public async Task GetProducten_ResponseIsJson()
    {
        var response = await _client.GetAsync("/api/producten");
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
    }

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

    [Fact]
    public async Task PutProduct_ReturnsNoContent_AndUpdatesProduct()
    {
        // Eerst een bestaand product ophalen
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

        // Controleren of update is doorgevoerd
        var getUpdatedResponse = await _client.GetAsync($"/api/producten/{productToUpdate.Id}");
        getUpdatedResponse.EnsureSuccessStatusCode();
        var updatedContent = await getUpdatedResponse.Content.ReadFromJsonAsync<ZorgProduct>();
        Assert.Equal("UpdatedNaam", updatedContent.Naam);
        Assert.Equal("UpdatedOmschrijving", updatedContent.Omschrijving);
        Assert.Equal("UpdatedType", updatedContent.Type);
        Assert.Equal(10, updatedContent.Kosten);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_AndProductIsDeleted()
    {
        // Eerst een product toevoegen dat we gaan verwijderen
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

        // DELETE request sturen
        var deleteResponse = await _client.DeleteAsync($"/api/producten/{createdProduct.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Controleren dat het product niet meer bestaat
        var getResponse = await _client.GetAsync($"/api/producten/{createdProduct.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
