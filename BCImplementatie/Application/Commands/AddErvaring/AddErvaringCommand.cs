using MediatR;

namespace BCImplementatie.Application.Commands.AddErvaring
{
    public record AddErvaringCommand(
        Guid GebruikId,
        DateTime Datum,
        string Review,
        string Observatie
    ) : IRequest<Domain.Entities.Ervaring>;
}
