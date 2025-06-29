using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZorgtechnologieProduct.Domain;
using ZorgtechnologieProduct.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ActionResult<IEnumerable<ZorgProduct>> GetProducten()
        {
            var producten = _context.Zorgproducten.ToList();
            return Ok(producten);
        }

        [HttpGet("{id}")]
        public ActionResult<ZorgProduct> GetProduct(Guid id)
        {
            var product = _context.Zorgproducten.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public ActionResult<ZorgProduct> PostProduct([FromBody] ZorgProduct product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            product.Id = Guid.NewGuid();
            _context.Zorgproducten.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
    }
}
