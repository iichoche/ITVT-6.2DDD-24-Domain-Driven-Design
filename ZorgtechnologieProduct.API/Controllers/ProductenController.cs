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
    }
}
