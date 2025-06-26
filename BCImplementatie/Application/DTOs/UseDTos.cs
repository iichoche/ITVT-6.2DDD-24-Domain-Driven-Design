using System;

namespace BCImplementatie.Application.DTOs
{
    public record StartGebruikDto(int ClientId, Guid ProductItemId);
    public record AddCareNeedDto(string NeedDescription, string NeedCategoryName, Guid? AdviesProductId);
    public record AddErvaringDto(DateTime Datum, string Review, string Observatie);

    // CreateCareNeedDto.cs
    public record CreateCareNeedDto(Guid GebruikId, string NeedDescription, string NeedCategoryName, Guid? AdviesProductId);

    // UpdateCareNeedDto.cs
    public record UpdateCareNeedDto(string NeedDescription, string NeedCategoryName, Guid? AdviesProductId);

    // CreateErvaringDto.cs
    public record CreateErvaringDto(Guid GebruikId, DateTime Datum, string Review, string Observatie);

    // UpdateErvaringDto.cs
    public record UpdateErvaringDto(DateTime Datum, string Review, string Observatie);

    // CreateGebruikDto.cs
    public record CreateGebruikDto(int ClientId, Guid ProductItemId);

    // UpdateGebruikDto.cs
    public record UpdateGebruikDto(bool InGebruik);

    public record ErvaringDto(
    int Id,
    Guid GebruikId,
    DateTime Datum,
    string Review,
    string Observatie
    );
}
