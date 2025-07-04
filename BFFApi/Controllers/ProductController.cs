using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using BFFApi.Models;


namespace BffApi.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly string _implBase;

        public ProductController(IHttpClientFactory factory, IConfiguration cfg)
        {
            _client = factory.CreateClient("BFF");
            _implBase = cfg["IMPLEMENTATIE_API_URL"]!;
        }

        // POST api/product/vrijgeven
        [HttpPost("vrijgeven")]
        public async Task<IActionResult> Release([FromBody] ReleaseProductDto dto)
        {
            var resp = await _client.PostAsJsonAsync($"{_implBase}/product/vrijgeven", dto);
            resp.EnsureSuccessStatusCode();
            return Created(string.Empty, await resp.Content.ReadFromJsonAsync<JsonElement>());
        }

        // POST api/product/{id}/inuse
        [HttpPost("{id}/inuse")]
        public async Task<IActionResult> InUse(string id, [FromBody] InUseProductDto dto)
        {
            var resp = await _client.PostAsJsonAsync($"{_implBase}/product/{id}/inuse", dto);
            resp.EnsureSuccessStatusCode();
            return NoContent();
        }
    }
}
