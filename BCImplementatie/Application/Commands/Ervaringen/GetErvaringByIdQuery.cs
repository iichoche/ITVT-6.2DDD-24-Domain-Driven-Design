using BCImplementatie.Domain.Entities;
using MediatR;

namespace BCImplementatie.Application.Commands.Ervaringen
{
    public record GetErvaringByIdQuery(int Id) : IRequest<Ervaring>;
}
