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
}