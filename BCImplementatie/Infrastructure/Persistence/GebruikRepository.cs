using BCImplementatie.Application.Interfaces;
using BCImplementatie.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BCImplementatie.Infrastructure.Persistence
{
    public class GebruikRepository : IGebruikRepository
    {
        private readonly ImplementatieDbContext _db;

        public GebruikRepository(ImplementatieDbContext db)
            => _db = db;

        public async Task AddAsync(Gebruik gebruik, CancellationToken ct = default)
        {
            _db.Gebruiken.Add(gebruik);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Gebruik gebruik, CancellationToken ct = default)
        {
            _db.Gebruiken.Update(gebruik);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<Gebruik> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.Gebruiken
                        .Include(g => g.CareNeeds)
                        .Include(g => g.Ervaringen)
                        .SingleOrDefaultAsync(g => g.Id == id, ct);

        public async Task<IEnumerable<Gebruik>> ListAllAsync(CancellationToken ct = default)
            => await _db.Gebruiken
                        .Include(g => g.CareNeeds)
                        .Include(g => g.Ervaringen)
                        .ToListAsync(ct);
    }
}
