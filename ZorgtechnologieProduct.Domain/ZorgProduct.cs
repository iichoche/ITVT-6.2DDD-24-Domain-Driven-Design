namespace ZorgtechnologieProduct.Domain
{
    public class ZorgProduct
    {
        public Guid Id { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public string Type { get; set; } // <-- maak hier string van
        public decimal Kosten { get; set; }
    }
}
