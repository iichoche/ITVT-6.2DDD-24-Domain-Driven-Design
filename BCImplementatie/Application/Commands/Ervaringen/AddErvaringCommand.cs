using MediatR;

namespace BCImplementatie.Application.Commands.Ervaringen
{
    public record AddErvaringCommand(
        Guid GebruikId,
        DateTime Datum,
        string Review,
        string Observatie
    ) : IRequest<Domain.Entities.Ervaring>;
}