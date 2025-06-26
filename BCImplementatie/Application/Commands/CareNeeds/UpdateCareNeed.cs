using BCImplementatie.Domain.Entities;
using MediatR;

namespace BCImplementatie.Application.Commands.CareNeeds
{
    public record UpdateCareNeedCommand(
        int Id,
        string NeedDescription,
        string NeedCategoryName,
        Guid? AdviesProductId
    ) : IRequest<CareNeed>;
}