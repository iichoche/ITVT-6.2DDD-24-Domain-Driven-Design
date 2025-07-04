using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using BFFApi.Models;


namespace BffApi.Controllers
{
    [ApiController]
    [Route("api/product-categories")]
    public class ProductCategoriesController : ControllerBase
    {
        // 🔑 Sla hier wél een HttpClient op, niet de factory
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public ProductCategoriesController(IHttpClientFactory factory, IConfiguration cfg)
        {
            _http = factory.CreateClient("BFF");         // HttpClient, niet IHttpClientFactory
            _baseUrl = cfg["IMPLEMENTATIE_API_URL"]!;      // bv. "https://.../api"
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategoriesDto>>> GetAll()
        {
            // 1) haal alle gebruikssessies op (met product-IDs)
            var uses = await _http
                .GetFromJsonAsync<List<GebruikDto>>($"{_baseUrl}/gebruik")
                ?? new List<GebruikDto>();

            // 2) haal alle care-needs op
            var needs = await _http
                .GetFromJsonAsync<List<CareNeedDto>>($"{_baseUrl}/care-needs")
                ?? new List<CareNeedDto>();

            // 3) haal alle categories op
            var categories = await _http
                .GetFromJsonAsync<List<NeedCategoryDto>>($"{_baseUrl}/needcategories")
                ?? new List<NeedCategoryDto>();

            // 4) per gebruikId de distinct category-names verzamelen
            var grouped = needs
                .GroupBy(n => n.GebruikId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(n => n.NeedCategoryName).Distinct().ToList()
                );

            // 5) map van productId → categorie-lijst
            var prodToCats = uses
                .Where(u => grouped.ContainsKey(u.Id))
                .GroupBy(u => u.ZorgtechnologieProductItemId)
                .Select(grp =>
                {
                    var allUseIds = grp.Select(u => u.Id);
                    var names = allUseIds.SelectMany(id => grouped[id]).Distinct();

                    var catDtos = names.Select(name =>
                        categories.FirstOrDefault(c => c.Name == name)
                        ?? new NeedCategoryDto(name, "Onbekende categorie")
                    );

                    return new ProductCategoriesDto(grp.Key, catDtos);
                })
                .ToList();

            return Ok(prodToCats);
        }
    }
}
