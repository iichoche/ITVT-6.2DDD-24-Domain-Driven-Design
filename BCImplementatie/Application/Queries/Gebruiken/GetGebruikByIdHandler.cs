using MediatR;
using BCImplementatie.Application.Interfaces;
using BCImplementatie.Domain.Entities;

namespace BCImplementatie.Application.Queries.Gebruiken
{
    public class GetGebruikByIdHandler : IRequestHandler<GetGebruikByIdQuery, Gebruik>
    {
        private readonly IGebruikRepository _repo;
        public GetGebruikByIdHandler(IGebruikRepository repo) => _repo = repo;

        public async Task<Gebruik> Handle(GetGebruikByIdQuery request, CancellationToken ct)
            => await _repo.GetByIdAsync(request.Id);
    }
}
