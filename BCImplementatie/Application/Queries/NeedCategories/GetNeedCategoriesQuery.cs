using System.Collections.Generic;                  // for IEnumerable<T>
using System.Threading;                            // for CancellationToken
using System.Threading.Tasks;                      // for Task<T>
using MediatR;                                     // for IRequestHandler<,>
using BCImplementatie.Infrastructure.Persistence;  // for ImplementatieDbContext
using BCImplementatie.Domain.ValueObjects;         // ← your NeedCategory lives here
using Microsoft.EntityFrameworkCore;               // for AsNoTracking(), ToListAsync()

namespace BCImplementatie.Application.Queries.NeedCategories
{
    public record GetNeedCategoriesQuery() : IRequest<IEnumerable<NeedCategory>>;
}
