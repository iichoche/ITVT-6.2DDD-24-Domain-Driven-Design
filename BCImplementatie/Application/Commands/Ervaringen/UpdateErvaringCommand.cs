using BCImplementatie.Domain.Entities;
using MediatR;
namespace BCImplementatie.Application.Commands.Ervaringen
{
    public record UpdateErvaringCommand(
        int Id,
        DateTime Datum,
        string Review,
        string Observatie
    ) : IRequest<Ervaring>;

}
