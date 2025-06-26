
using System;
using System.Collections.Generic;

namespace ZorgtechnologieProduct.Domain
{
    public class ZorgtechnologieProduct
    {
    public Guid Id { get; set; }
    public required string Naam { get; set; }
    public required string Omschrijving { get; set; }
    public required string Type { get; set; }
    public required string Eigenschappen { get; set; }
    }
}
