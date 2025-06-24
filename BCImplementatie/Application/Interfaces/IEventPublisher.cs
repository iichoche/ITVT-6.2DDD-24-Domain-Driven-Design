using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BCImplementatie.Application.Interfaces
{
    /// <summary>
    /// A simple abstraction over MediatR to publish domain events.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publish a domain notification/event.
        /// </summary>
        Task PublishAsync(INotification @event, CancellationToken ct = default);
    }
}
