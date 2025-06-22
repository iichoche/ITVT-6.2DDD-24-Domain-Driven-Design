namespace BCImplementatie.Domain.ValueObjects
{
    public class NeedCategory
    {
        public string Name { get; private set; }        // PK
        public string Description { get; private set; }

        // EF Core requires a parameterless constructor. Initialize properties with default values.
        private NeedCategory()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        public NeedCategory(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
