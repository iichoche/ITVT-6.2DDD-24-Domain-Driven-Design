namespace BCImplementatie.Domain.Entities
{
    public class Ervaring
    {
    public int Id { get; set; }
    public Guid GebruikId { get; set; }           // FK toevoegen
    public DateTime Datum { get; set; }
    public string Review { get; set; }
    public string Observatie { get; set; }

        // Optioneel: // Navigatie terug naar parent Gebruik
        public Gebruik Gebruik { get; set; }
    }
} 