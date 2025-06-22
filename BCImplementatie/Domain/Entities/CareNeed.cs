using BCImplementatie.Domain.ValueObjects;

namespace BCImplementatie.Domain.Entities
{
    public class CareNeed
    {
        public int Id { get; set; }
        public Guid GebruikId { get; set; }  // FK naar Gebruik
        public int ClientId { get; set; }
        public string NeedDescription { get; set; }

        // 1) Hernoem deze property zodat hij overeenkomt met de Fluent API:
        public string NeedCategoryName { get; set; }

        public Guid? AdviesZorgtechnologieProductId { get; set; }

        // 2) Voeg de navigatie-property toe:
        public NeedCategory Category { get; set; }

        // 3) Navigatie terug naar Gebruik (al aanwezig):
        public Gebruik Gebruik { get; set; }
    }
}