using System;
using System.Collections.Generic;
using DomainProduct = ZorgtechnologieProduct.Domain.ZorgProduct;  // Alias aangepast

namespace ZorgtechnologieProduct.Application
{
    // Interface voor productservice (hiermee kun je producten ophalen of toevoegen, zoals Teun vroeg tijdens de demo)
    public interface IProductService
    {
        IEnumerable<DomainProduct> GetAll();         // Haal alle producten op
        DomainProduct GetById(Guid id);              // Haal één product op via id
        DomainProduct Create(DomainProduct product); // Maak een nieuw product aan
    }

    // Simpele implementatie van de productservice (in-memory lijst)
    public class ProductService : IProductService
    {
        private readonly List<DomainProduct> _producten = new();

        // Geeft alle producten terug
        public IEnumerable<DomainProduct> GetAll() => _producten;

        // Geeft een product terug op basis van id
        public DomainProduct GetById(Guid id) => _producten.Find(p => p.Id == id);

        // Maakt een nieuw product aan en voegt toe aan de lijst
        public DomainProduct Create(DomainProduct product)
        {
            product.Id = Guid.NewGuid();
            _producten.Add(product);
            return product;
        }
    }
}
