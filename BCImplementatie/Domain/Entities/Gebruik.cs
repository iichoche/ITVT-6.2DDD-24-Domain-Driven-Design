using System;
using System.Collections.Generic;

namespace BCImplementatie.Domain.Entities
{
    public class Gebruik
    {
        // ✏️ Voeg deze property toe:
        public Guid Id { get; private set; }

        public int ClientId { get; private set; }
        public Guid ZorgtechnologieProductItemId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public bool InGebruik { get; private set; }

        public List<CareNeed> CareNeeds { get; private set; } = new();
        public List<Ervaring> Ervaringen { get; private set; } = new();

        //  // Parameterloze constructor voor EF Core
        protected Gebruik() { }

        // Constructor
        public Gebruik(int clientId, Guid zorgtechnologieProductItemId)
        {
            Id = Guid.NewGuid();               // initialiseer de sleutel
            ClientId = clientId;
            ZorgtechnologieProductItemId = zorgtechnologieProductItemId;
            StartTime = DateTime.UtcNow;
            InGebruik = true;
        }

        // Methoden
        public void VoegCareNeedToe(CareNeed need) => CareNeeds.Add(need);
        public void VoegErvaringToe(Ervaring ervaring) => Ervaringen.Add(ervaring);
        public void StopGebruik() => (InGebruik, EndTime) = (false, DateTime.UtcNow);
    }
}