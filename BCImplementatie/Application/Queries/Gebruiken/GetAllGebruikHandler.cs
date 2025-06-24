using MediatR;
using BCImplementatie.Application.Interfaces;
using BCImplementatie.Domain.Entities;


namespace BCImplementatie.Application.Queries.Gebruiken
{
    public class GetAllGebruikHandler : IRequestHandler<GetAllGebruikQuery, IEnumerable<Gebruik>>
    {
        private readonly IGebruikRepository _repo;
        public GetAllGebruikHandler(IGebruikRepository repo) => _repo = repo;

        public async Task<IEnumerable<Gebruik>> Handle(GetAllGebruikQuery request, CancellationToken ct)
        {
            return await _repo.ListAllAsync(ct);
        }
    }
}
