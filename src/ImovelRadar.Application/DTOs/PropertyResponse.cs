namespace ImovelRadar.Application.DTOs;

public class PropertyResponse
{
    public int Id { get; init; }
    public string? Title { get; init; }

    public string PropertyType { get; init; } = null!;
    public string TransactionType { get; init; } = null!;

    public decimal Price { get; init; }
    public decimal? Area { get; init; }
    public decimal? PricePerSquareMeter { get; init; }

    public int? Bedrooms { get; init; }
    public int? Bathrooms { get; init; }
    public int? ParkingSpaces { get; init; }

    public string? Neighborhood { get; init; }
    public string City { get; init; } = null!;
    public string State { get; init; } = null!;

    public string? SourceUrl { get; init; }

    public DateTime FirstSeenAt { get; init; }
    public DateTime LastSeenAt { get; init; }
}