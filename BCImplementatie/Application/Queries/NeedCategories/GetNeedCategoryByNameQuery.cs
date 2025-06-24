using MediatR;
using BCImplementatie.Domain.ValueObjects;         // ← your NeedCategory lives here

namespace BCImplementatie.Application.Queries.NeedCategories
{
    // Renaming the record to avoid CS0101 (duplicate definition) error
    public record GetNeedCategoryByNameQueryRequest(string Name) : IRequest<NeedCategory>
    {
        // Explicitly initializing the property to avoid CS8907 (unread parameter) error
        public string Name { get; init; } = Name;
    }
}
