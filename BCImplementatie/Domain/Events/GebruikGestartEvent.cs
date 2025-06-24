using MediatR;
using System;

namespace BCImplementatie.Domain.Events
{
    /// <summary>
    /// Raised when a new Gebruik (session) is started.
    /// </summary>
    public class GebruikGestartEvent : INotification
    {
        public Guid GebruikId { get; }

        // 🔧 Add this constructor:
        public GebruikGestartEvent(Guid gebruikId)
        {
            GebruikId = gebruikId;
        }
    }
}
