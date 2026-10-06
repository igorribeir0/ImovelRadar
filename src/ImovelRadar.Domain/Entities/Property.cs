using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Entities;

public class Property
{
    public Property(
    string? title,
    PropertyType propertyType,
    TransactionType transactionType,
    decimal price,
    decimal? area,
    int? bedrooms,
    int? bathrooms,
    int? parkingSpaces,
    string city,
    string state,
    int sourceId,
    string? neighborhood = null,
    string? externalId = null,
    string? sourceUrl = null)
{
    if (price <= 0)
        throw new ArgumentOutOfRangeException(
            nameof(price),
            "O preço do imóvel deve ser maior que zero.");

    if (area is <= 0)
        throw new ArgumentOutOfRangeException(
            nameof(area),
            "A área do imóvel deve ser maior que zero.");

    if (string.IsNullOrWhiteSpace(city))
        throw new ArgumentException(
            "A cidade é obrigatória.",
            nameof(city));

    if (string.IsNullOrWhiteSpace(state))
        throw new ArgumentException(
            "O estado é obrigatório.",
            nameof(state));

    if (sourceId <= 0)
        throw new ArgumentOutOfRangeException(
            nameof(sourceId),
            "A origem do imóvel é obrigatória.");

    Title = title;
    PropertyType = propertyType;
    TransactionType = transactionType;
    Price = price;
    Area = area;
    Bedrooms = bedrooms;
    Bathrooms = bathrooms;
    ParkingSpaces = parkingSpaces;
    City = city.Trim();
    State = state.Trim();
    SourceId = sourceId;

    Neighborhood = string.IsNullOrWhiteSpace(neighborhood)
    ? null
    : neighborhood.Trim();

    ExternalId = externalId;
    SourceUrl = sourceUrl;

    var now = DateTime.UtcNow;

    FirstSeenAt = now;
    LastSeenAt = now;
    CreatedAt = now;
    UpdatedAt = now;
}

    public int Id { get; private set; }

    public string? Title { get; private set; }

    public PropertyType PropertyType { get; private set; }

    public TransactionType TransactionType { get; private set; }

    public decimal Price { get; private set; }

    public decimal? Area { get; private set; }

    public int? Bedrooms { get; private set; }

    public int? Bathrooms { get; private set; }

    public int? ParkingSpaces { get; private set; }

    public decimal? CondoFee { get; private set; }

    public decimal? Iptu { get; private set; }

    public string? Address { get; private set; }

    public string? Neighborhood { get; private set; }

    public string City { get; private set; } = null!;

    public string State { get; private set; } = null!;

    public string? PostalCode { get; private set; }

    public decimal? Latitude { get; private set; }

    public decimal? Longitude { get; private set; }

    public int SourceId { get; private set; }

    public string? ExternalId { get; private set; }

    public string? SourceUrl { get; private set; }

    public DateTime FirstSeenAt { get; private set; }

    public DateTime LastSeenAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }


public decimal? GetPricePerSquareMeter()
{
    if (Area is null || Area <= 0)
        return null;

    return Price / Area.Value;
}


public void UpdatePrice(decimal newPrice)
{
    if (newPrice <= 0)
        throw new ArgumentOutOfRangeException(
            nameof(newPrice),
            "O novo preço do imóvel deve ser maior que zero.");

    if (newPrice == Price)
        return;

    Price = newPrice;
    UpdatedAt = DateTime.UtcNow;
}

public void AssignId(int id)
{
    if (id <= 0)
    {
        throw new ArgumentOutOfRangeException(
            nameof(id),
            "O identificador do imóvel deve ser maior que zero.");
    }

    if (Id != 0)
    {
        throw new InvalidOperationException(
            "O imóvel já possui um identificador.");
    }

    Id = id;
}

}