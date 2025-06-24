using System.Collections.Generic;                  // for IEnumerable<T>
using System.Threading;                            // for CancellationToken
using System.Threading.Tasks;                      // for Task<T>
using MediatR;                                     // for IRequestHandler<,>
using BCImplementatie.Infrastructure.Persistence;  // for ImplementatieDbContext
using BCImplementatie.Domain.ValueObjects;         // ← your NeedCategory lives here
using Microsoft.EntityFrameworkCore;               // for AsNoTracking(), ToListAsync()

namespace BCImplementatie.Application.Queries.NeedCategories
{
    public class GetNeedCategoryByNameHandler : IRequestHandler<GetNeedCategoryByNameQuery, NeedCategory>
    {
        private readonly ImplementatieDbContext _db;
        public GetNeedCategoryByNameHandler(ImplementatieDbContext db) => _db = db;

        public async Task<NeedCategory> Handle(GetNeedCategoryByNameQuery request, CancellationToken ct)
            => await _db.NeedCategories.SingleOrDefaultAsync(n => n.Name == request.Name, ct);
    }

    // Fix: Ensure GetNeedCategoryByNameQuery implements IRequest<NeedCategory>
    public record GetNeedCategoryByNameQuery : IRequest<NeedCategory>
    {
        public string Name { get; init; }
    }
}
