using BCImplementatie.Domain.Entities;
using MediatR;

namespace BCImplementatie.Application.Queries.CareNeeds
{
    public record GetCareNeedByIdQuery(int Id) : IRequest<CareNeed>;
}
