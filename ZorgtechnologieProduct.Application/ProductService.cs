using System;
using System.Collections.Generic;
using DomainProduct = ZorgtechnologieProduct.Domain.ZorgProduct;  // Alias aangepast

namespace ZorgtechnologieProduct.Application
{
    public interface IProductService
    {
        IEnumerable<DomainProduct> GetAll();
        DomainProduct GetById(Guid id);
        DomainProduct Create(DomainProduct product);
    }

    public class ProductService : IProductService
    {
        private readonly List<DomainProduct> _producten = new();

        public IEnumerable<DomainProduct> GetAll() => _producten;

        public DomainProduct GetById(Guid id) => _producten.Find(p => p.Id == id);

        public DomainProduct Create(DomainProduct product)
        {
            product.Id = Guid.NewGuid();
            _producten.Add(product);
            return product;
        }
    }
}
