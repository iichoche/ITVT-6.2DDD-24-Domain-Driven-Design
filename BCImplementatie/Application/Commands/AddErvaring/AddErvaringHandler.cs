using MediatR;
using BCImplementatie.Domain.Entities;
using BCImplementatie.Application.Interfaces;

namespace BCImplementatie.Application.Commands.AddErvaring
{
    public class AddErvaringHandler : IRequestHandler<AddErvaringCommand, Ervaring>
    {
        private readonly IGebruikRepository _repo;

        public AddErvaringHandler(IGebruikRepository repo) => _repo = repo;

        public async Task<Ervaring> Handle(AddErvaringCommand cmd, CancellationToken ct)
        {
            var gebruik = await _repo.GetByIdAsync(cmd.GebruikId);
            var erv = new Ervaring
            {
                GebruikId = cmd.GebruikId,
                Datum = cmd.Datum,
                Review = cmd.Review,
                Observatie = cmd.Observatie
            };
            gebruik.VoegErvaringToe(erv);
            await _repo.UpdateAsync(gebruik);
            return erv;
        }
    }
}
