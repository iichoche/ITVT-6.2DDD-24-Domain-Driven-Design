using MediatR;
using System.Collections.Generic;
using BCImplementatie.Domain.Entities;

namespace BCImplementatie.Application.Queries.Gebruiken
{
    // This record must implement IRequest<IEnumerable<Gebruik>>
    public record GetAllGebruikQuery()
        : IRequest<IEnumerable<Gebruik>>;
}