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

        [HttpGet]
        public ActionResult<IEnumerable<ZorgProductMetStatus>> GetProducten()
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

        [HttpGet("{id}")]
        public ActionResult<ZorgProduct> GetProduct(Guid id)
        {
            var product = _context.Zorgproducten.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // PUT endpoint om een product te updaten
        [HttpPut("{id}")]
        public ActionResult PutProduct(Guid id, [FromBody] ZorgProduct product)
        {
            if (product == null || id != product.Id)
            {
                return BadRequest();
            }

            var bestaandProduct = _context.Zorgproducten.FirstOrDefault(p => p.Id == id);
            if (bestaandProduct == null)
            {
                return NotFound();
            }

            // Update properties
            bestaandProduct.Naam = product.Naam;
            bestaandProduct.Omschrijving = product.Omschrijving;
            bestaandProduct.Kosten = product.Kosten;
            bestaandProduct.Type = product.Type;

            _context.SaveChanges();

            return NoContent();
        }

        // PATCH endpoint om 'InGebruik' te togglen of instellen
        [HttpPatch("{id}/gebruik")]
        public ActionResult ToggleInGebruik(Guid id, [FromBody] bool inGebruik)
        {
            // Check of er al een item bestaat
            var item = _context.ZorgtechnologieProductItems.FirstOrDefault(i => i.ZorgtechnologieProductId == id);
            if (item == null)
            {
                // Maak nieuw item aan als niet bestaat
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

        // DELETE endpoint om product te verwijderen (incl. eventuele items)
        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(Guid id)
        {
            var product = _context.Zorgproducten.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            // Verwijder bijbehorende items
            var items = _context.ZorgtechnologieProductItems.Where(i => i.ZorgtechnologieProductId == id);
            _context.ZorgtechnologieProductItems.RemoveRange(items);

            // Verwijder product
            _context.Zorgproducten.Remove(product);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
