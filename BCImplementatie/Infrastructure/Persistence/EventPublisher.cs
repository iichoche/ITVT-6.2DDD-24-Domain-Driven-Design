using System.Threading;
using System.Threading.Tasks;
using MediatR;
using BCImplementatie.Application.Interfaces;

namespace BCImplementatie.Infrastructure.Persistence
{
    /// <summary>
    /// Forwards domain events into MediatR’s pipeline.
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IMediator _mediator;
        public EventPublisher(IMediator mediator) => _mediator = mediator;

        public Task PublishAsync(INotification @event, CancellationToken ct = default)
            => _mediator.Publish(@event, ct);
    }
}