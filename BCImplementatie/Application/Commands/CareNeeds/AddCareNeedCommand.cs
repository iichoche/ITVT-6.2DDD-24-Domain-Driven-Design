using BCImplementatie.Domain.Entities;
using MediatR;

namespace BCImplementatie.Application.Commands.CareNeeds
{
    public record AddCareNeedCommand(
        Guid GebruikId,
        string NeedDescription,
        string NeedCategoryName,
        Guid? AdviesProductId
    ) : IRequest<CareNeed>;
}
