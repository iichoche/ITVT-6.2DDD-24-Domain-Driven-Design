namespace ZorgtechnologieProduct.API.DTO
{
    public class ZorgProductMetStatus
    {
        public Guid Id { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public string Type { get; set; }
        public decimal Kosten { get; set; }
        public bool InGebruik { get; set; }
    }
}
