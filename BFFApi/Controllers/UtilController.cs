using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using BFFApi.Models;


namespace BffApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilController : ControllerBase
    {
        private readonly IHttpClientFactory _factory;
        private readonly string _implUrl;
        private readonly string _zorgUrl;
        private readonly string _clientUrl;

        public UtilController(IHttpClientFactory factory, IConfiguration cfg)
        {
            _factory   = factory;
            _implUrl   = cfg["IMPLEMENTATIE_API_URL"]!;
            _zorgUrl   = cfg["ZORGTECH_API_URL"]!;
            _clientUrl = cfg["CLIENT_API_URL"]!;
        }

        [HttpPost("registreerGebruik")]
        public async Task<IActionResult> RegistreerGebruik([FromBody] RegistreerGebruikRequest req)
        {
            var client = _factory.CreateClient("BFF");

            // 1) productItemId ophalen
            var prod = await client.GetFromJsonAsync<ProductDto>(
                $"{_zorgUrl}/zorgtechnologieproduct-item/haalOp?naam={Uri.EscapeDataString(req.ProductNaam)}");

            // 2) clientId ophalen
            var cli = await client.GetFromJsonAsync<ClientDto>(
                $"{_clientUrl}/client/haalOp?naam={Uri.EscapeDataString(req.ClientNaam)}");

            // 3) nieuw gebruik aanmaken
            var body = new { clientId = cli!.ClientId, productItemId = prod!.ProductItemId, config = req.Config };
            var resp = await client.PostAsJsonAsync($"{_implUrl}/gebruik", body);

            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadFromJsonAsync<RegistreerGebruikResponse>();
            return Created(string.Empty, result);
        }
    }
}
