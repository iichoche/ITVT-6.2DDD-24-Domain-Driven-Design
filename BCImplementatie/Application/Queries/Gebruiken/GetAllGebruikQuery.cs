using BCImplementatie.Domain.Entities;
using MediatR;

namespace BCImplementatie.Application.Queries.Gebruiken
{
    // This record must implement IRequest<IEnumerable<Gebruik>>
    public record GetAllGebruikQuery()
        : IRequest<IEnumerable<Gebruik>>;

    public record GetGebruikByIdQuery(Guid Id)
        : IRequest<Gebruik>;

}