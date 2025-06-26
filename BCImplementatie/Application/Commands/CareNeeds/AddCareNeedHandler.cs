using MediatR;
using BCImplementatie.Domain.Entities;
using BCImplementatie.Application.Interfaces;

namespace BCImplementatie.Application.Commands.CareNeeds
{
    public class AddCareNeedHandler : IRequestHandler<AddCareNeedCommand, CareNeed>
    {
        private readonly IGebruikRepository _repo;

        public AddCareNeedHandler(IGebruikRepository repo) => _repo = repo;

        public async Task<CareNeed> Handle(AddCareNeedCommand cmd, CancellationToken ct)
        {
            var gebruik = await _repo.GetByIdAsync(cmd.GebruikId);
            var need = new CareNeed
            {
                GebruikId = cmd.GebruikId,
                NeedDescription = cmd.NeedDescription,
                NeedCategoryName = cmd.NeedCategoryName,
                AdviesZorgtechnologieProductId = cmd.AdviesProductId
            };
            gebruik.VoegCareNeedToe(need);
            await _repo.UpdateAsync(gebruik);
            return need;
        }
    }
}
