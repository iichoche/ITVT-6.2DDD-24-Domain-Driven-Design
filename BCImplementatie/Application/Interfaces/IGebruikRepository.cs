using BCImplementatie.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BCImplementatie.Application.Interfaces
{
    public interface IGebruikRepository
    {
        Task AddAsync(Gebruik gebruik, CancellationToken ct = default);
        Task UpdateAsync(Gebruik gebruik, CancellationToken ct = default);
        Task<Gebruik> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Gebruik>> ListAllAsync(CancellationToken ct = default);
    }
}
