using System.Collections.Generic;                  // for IEnumerable<T>
using System.Threading;                            // for CancellationToken
using System.Threading.Tasks;                      // for Task<T>
using MediatR;                                     // for IRequestHandler<,>
using BCImplementatie.Infrastructure.Persistence;  // for ImplementatieDbContext
using BCImplementatie.Domain.ValueObjects;         // ← your NeedCategory lives here
using Microsoft.EntityFrameworkCore;               // for AsNoTracking(), ToListAsync()


namespace BCImplementatie.Application.Queries.NeedCategories
{
    public class GetNeedCategoriesHandler : IRequestHandler<GetNeedCategoriesQuery, IEnumerable<NeedCategory>>
    {
        private readonly ImplementatieDbContext _db;
        public GetNeedCategoriesHandler(ImplementatieDbContext db) => _db = db;

        public async Task<IEnumerable<NeedCategory>> Handle(GetNeedCategoriesQuery request, CancellationToken ct)
            => await _db.NeedCategories.AsNoTracking().ToListAsync(ct);
    }
}
