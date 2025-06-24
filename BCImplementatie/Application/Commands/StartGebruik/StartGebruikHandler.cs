using MediatR;
using BCImplementatie.Domain.Entities;
using BCImplementatie.Domain.Events;
using BCImplementatie.Application.Interfaces;

namespace BCImplementatie.Application.Commands.StartGebruik
{
    public class StartGebruikHandler : IRequestHandler<StartGebruikCommand, Guid>
    {
        private readonly IGebruikRepository _repo;
        private readonly IEventPublisher _publisher;

        public StartGebruikHandler(IGebruikRepository repo, IEventPublisher publisher)
            => (_repo, _publisher) = (repo, publisher);

        public async Task<Guid> Handle(StartGebruikCommand cmd, CancellationToken ct)
        {
            var g = new Gebruik(cmd.ClientId, cmd.ProductItemId);
            await _repo.AddAsync(g);
            await _publisher.PublishAsync(new GebruikGestartEvent(g.Id));
            return g.Id;
        }
    }
}
