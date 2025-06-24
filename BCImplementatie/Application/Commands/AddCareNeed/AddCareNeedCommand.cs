using MediatR;

namespace BCImplementatie.Application.Commands.AddCareNeed
{
    public record AddCareNeedCommand(
        Guid GebruikId,
        string NeedDescription,
        string NeedCategoryName,
        Guid? AdviesProductId
    ) : IRequest<Domain.Entities.CareNeed>;
}
