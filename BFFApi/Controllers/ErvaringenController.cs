using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;

namespace BffApi.Controllers
{
    [ApiController]
    [Route("api/gebruik/{gebruikId}/ervaringen")]
    public class BffErvaringenController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly string _implBase;

        public BffErvaringenController(IHttpClientFactory factory, IConfiguration cfg)
        {
            _client = factory.CreateClient("BFF");
            _implBase = cfg["IMPLEMENTATIE_API_URL"]!;
        }

        // POST api/gebruik/{gebruikId}/ervaringen
        [HttpPost]
        public async Task<IActionResult> Register(string gebruikId, [FromBody] CreateErvaringDto dto)
        {
            var resp = await _client.PostAsJsonAsync(
                $"{_implBase}/gebruik/{gebruikId}/ervaringen", dto);
            resp.EnsureSuccessStatusCode();
            return Created(string.Empty, await resp.Content.ReadFromJsonAsync<JsonElement>());
        }
    }

    // --- DTO voor Ervaring ---
    public record CreateErvaringDto(
        DateTime Datum,
        string Review,
        string Observatie
    );
}
