using BCImplementatie.Domain.Entities;
using MediatR;

namespace BCImplementatie.Application.Queries.CareNeeds
{
    public record GetAllCareNeedsQuery() : IRequest<IEnumerable<CareNeed>>;
}

