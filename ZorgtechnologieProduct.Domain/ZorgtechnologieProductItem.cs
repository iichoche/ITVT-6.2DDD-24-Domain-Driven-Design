namespace ZorgtechnologieProduct.Domain
{
    public class ZorgtechnologieProductItem
    {
        public Guid Id { get; set; }
        public Guid ZorgtechnologieProductId { get; set; }
        public DateTime? AanschafDatum { get; set; }
        public decimal? AanschafKosten { get; set; }
        public bool InGebruik { get; set; }
    }
}
