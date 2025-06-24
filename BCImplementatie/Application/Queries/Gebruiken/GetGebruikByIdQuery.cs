using MediatR;
using BCImplementatie.Domain.Entities;

namespace BCImplementatie.Application.Queries.Gebruiken
{
    public record GetGebruikByIdQuery(Guid Id) : IRequest<Gebruik>;
}
