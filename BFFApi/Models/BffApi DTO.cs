using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BFFApi.Models
{
    #region Zorgbehoefte DTOs

    /// <summary>
    /// Verzoek om een nieuwe zorgbehoefte te registreren.
    /// </summary>
    public record CreateZorgBehoefteDto(
        Guid GebruikId,
        string NeedDescription,
        string NeedCategoryName,
        Guid? AdviesProductId
    );

    /// <summary>
    /// Verzoek om een zorgbehoefte te classificeren.
    /// </summary>
    public record ClassifyZorgBehoefteDto(
        string CategoryName
    );

    /// <summary>
    /// Verzoek om een bestaande zorgbehoefte bij te werken.
    /// </summary>
    public record UpdateZorgBehoefteDto(
        string NeedDescription,
        string NeedCategoryName,
        Guid? AdviesProductId
    );

    #endregion

    #region UI-Controller DTOs

    /// <summary>
    /// Verzoek van de UI om een nieuw gebruik te registreren.
    /// </summary>
    public record RegistreerGebruikRequest(
        string ProductNaam,
        string ClientNaam,
        JsonElement Config
    );

    /// <summary>
    /// Respons na registratie van een nieuw gebruik.
    /// </summary>
    public record RegistreerGebruikResponse(
        Guid GebruikId,
        int ClientId,
        Guid ProductItemId
    );

    /// <summary>
    /// Representatie van een product-item.
    /// </summary>
    public record ProductDto(
        Guid ProductItemId
    );

    /// <summary>
    /// Representatie van een client.
    /// </summary>
    public record ClientDto(
        int ClientId
    );

    #endregion

    #region Product DTOs

    /// <summary>
    /// Verzoek om een product vrij te geven.
    /// </summary>
    public record ReleaseProductDto(
        Guid ProductItemId,
        int ClientId
    );

    /// <summary>
    /// Status van een product-in-gebruik.
    /// </summary>
    public record InUseProductDto(
        DateTime StartTime,
        bool InGebruik
    );

    #endregion

    #region Aggregated/Lookup DTOs

    /// <summary>
    /// Eenvoudige representatie van een gebruik-sessie met product-ID.
    /// </summary>
    public record GebruikDto(
        Guid Id,
        Guid ZorgtechnologieProductItemId
    );

    /// <summary>
    /// Eenvoudige representatie van een care-need.
    /// </summary>
    public record CareNeedDto(
        int Id,
        Guid GebruikId,
        string NeedDescription,
        string NeedCategoryName,
        Guid? AdviesZorgtechnologieProductItemId
    );

    /// <summary>
    /// Lookup-object voor een categorie.
    /// </summary>
    public record NeedCategoryDto(
        string Name,
        string Description
    );

    /// <summary>
    /// Aggregatie per product: alle bijbehorende categorieën.
    /// </summary>
    public record ProductCategoriesDto(
        Guid ProductId,
        IEnumerable<NeedCategoryDto> Categories
    );

    #endregion

    #region Ervaring DTOs

    /// <summary>
    /// Verzoek om een nieuwe gebruikerservaring te registreren.
    /// </summary>
    public record CreateErvaringDto(
        DateTime Datum,
        string Review,
        string Observatie
    );

    #endregion
}
