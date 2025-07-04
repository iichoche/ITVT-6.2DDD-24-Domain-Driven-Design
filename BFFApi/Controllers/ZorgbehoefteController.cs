using Microsoft.AspNetCore.Mvc;
using BFFApi.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;

namespace BffApi.Controllers
{
    [ApiController]
    [Route("api/zorgbehoefte")]
    public class ZorgBehoefteController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly string _implBase;

        public ZorgBehoefteController(IHttpClientFactory factory, IConfiguration cfg)
        {
            _client = factory.CreateClient("BFF");
            _implBase = cfg["IMPLEMENTATIE_API_URL"]!;
        }

        // POST api/zorgbehoefte
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateZorgBehoefteDto dto)
        {
            var resp = await _client.PostAsJsonAsync($"{_implBase}/zorgbehoefte", dto);
            resp.EnsureSuccessStatusCode();
            return Created(string.Empty, await resp.Content.ReadFromJsonAsync<JsonElement>());
        }

        // PUT api/zorgbehoefte/{id}/classificatie
        [HttpPut("{id}/classificatie")]
        public async Task<IActionResult> Classify(string id, [FromBody] ClassifyZorgBehoefteDto dto)
        {
            var resp = await _client.PutAsJsonAsync($"{_implBase}/zorgbehoefte/{id}/classificatie", dto);
            resp.EnsureSuccessStatusCode();
            return NoContent();
        }

        // PUT api/zorgbehoefte/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateZorgBehoefteDto dto)
        {
            var resp = await _client.PutAsJsonAsync($"{_implBase}/zorgbehoefte/{id}", dto);
            resp.EnsureSuccessStatusCode();
            return NoContent();
        }

        // DELETE api/zorgbehoefte/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var resp = await _client.DeleteAsync($"{_implBase}/zorgbehoefte/{id}");
            resp.EnsureSuccessStatusCode();
            return NoContent();
        }
    }
}
