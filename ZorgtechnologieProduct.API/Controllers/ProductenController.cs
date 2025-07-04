using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZorgtechnologieProduct.Domain;
using ZorgtechnologieProduct.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using ZorgtechnologieProduct.API.DTO;

namespace ZorgtechnologieProduct.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductenController : ControllerBase
    {
        private readonly ZorgtechnologieProductDbContext _context;
        private readonly ILogger<ProductenController> _logger;

        public ProductenController(ZorgtechnologieProductDbContext context, ILogger<ProductenController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Haal alle producten op, inclusief status (in gebruik of niet)
        [HttpGet]
        public ActionResult<IEnumerable<ZorgProductMetStatus>> GetProducten()
        {
            try
            {
                var producten = from product in _context.Zorgproducten
                                join item in _context.ZorgtechnologieProductItems
                                    on product.Id equals item.ZorgtechnologieProductId into itemGroup
                                from item in itemGroup.DefaultIfEmpty()
                                select new ZorgProductMetStatus
                                {
                                    Id = product.Id,
                                    Naam = product.Naam,
                                    Omschrijving = product.Omschrijving,
                                    Type = product.Type,
                                    Kosten = product.Kosten,
                                    InGebruik = item != null && item.InGebruik
                                };

                return Ok(producten.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij ophalen van producten");
                return StatusCode(500, $"Interne serverfout: {ex.Message}");
            }
        }

        // Voeg een nieuw product toe
        [HttpPost]
        public ActionResult<ZorgProduct> PostProduct([FromBody] ZorgProduct product)
        {
            if (product == null)
                return BadRequest();

            product.Id = Guid.NewGuid();
            _context.Zorgproducten.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // Haal één product op via id
        [HttpGet("{id}")]
        public ActionResult<ZorgProduct> GetProduct(Guid id)
        {
            var product = _context.Zorgproducten.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // Pas een bestaand product aan
        [HttpPut("{id}")]
        public ActionResult PutProduct(Guid id, [FromBody] ZorgProduct product)
        {
            if (product == null || id != product.Id)
                return BadRequest();

            var bestaandProduct = _context.Zorgproducten.FirstOrDefault(p => p.Id == id);
            if (bestaandProduct == null)
                return NotFound();

            bestaandProduct.Naam = product.Naam;
            bestaandProduct.Omschrijving = product.Omschrijving;
            bestaandProduct.Kosten = product.Kosten;
            bestaandProduct.Type = product.Type;

            _context.SaveChanges();

            return NoContent();
        }

        // Zet een product in of uit gebruik
        [HttpPatch("{id}/gebruik")]
        public ActionResult ToggleInGebruik(Guid id, [FromBody] bool inGebruik)
        {
            var item = _context.ZorgtechnologieProductItems.FirstOrDefault(i => i.ZorgtechnologieProductId == id);
            if (item == null)
            {
                item = new ZorgtechnologieProductItem
                {
                    Id = Guid.NewGuid(),
                    ZorgtechnologieProductId = id,
                    InGebruik = inGebruik
                };
                _context.ZorgtechnologieProductItems.Add(item);
            }
            else
            {
                item.InGebruik = inGebruik;
                _context.ZorgtechnologieProductItems.Update(item);
            }
            _context.SaveChanges();

            return NoContent();
        }

        // Verwijder een product (en bijbehorende items)
        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(Guid id)
        {
            var product = _context.Zorgproducten.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            var items = _context.ZorgtechnologieProductItems.Where(i => i.ZorgtechnologieProductId == id);
            _context.ZorgtechnologieProductItems.RemoveRange(items);

            _context.Zorgproducten.Remove(product);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
