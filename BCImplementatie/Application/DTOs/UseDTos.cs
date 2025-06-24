using System;

namespace BCImplementatie.Application.DTOs
{
    public record StartGebruikDto(int ClientId, Guid ProductItemId);
    public record AddCareNeedDto(string NeedDescription, string NeedCategoryName, Guid? AdviesProductId);
    public record AddErvaringDto(DateTime Datum, string Review, string Observatie);
}
