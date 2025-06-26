using BCImplementatie.Domain.Entities;
using MediatR;

namespace BCImplementatie.Application.Commands.Ervaringen
{ 
    public record GetAllErvaringenQuery() : IRequest<IEnumerable<Ervaring>>;
}
