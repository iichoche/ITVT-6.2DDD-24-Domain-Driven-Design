using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ZorgtechnologieProduct.Application;
using ZorgtechnologieProduct.Domain;

namespace ZorgtechnologieProduct.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IZorgtechnologieZoekerService _zoekerService;

        public ProductController(IZorgtechnologieZoekerService zoekerService)
        {
            _zoekerService = zoekerService;
        }

        [HttpGet]
        public ActionResult<List<Domain.ZorgtechnologieProduct>> Get()
        {
            var producten = _zoekerService.FilterOpCriteria(new ZorgtechnologieFilter());
            return Ok(producten);
        }

        [HttpGet("{id}")]
        public ActionResult<Domain.ZorgtechnologieProduct> Get(Guid id)
        {
            var product = _zoekerService.GeefDetails(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpGet("{id}/beschikbaar")]
        public ActionResult<bool> CheckBeschikbaarheid(Guid id)
        {
            return Ok(_zoekerService.CheckBeschikbaarheid(id));
        }
    }
}
