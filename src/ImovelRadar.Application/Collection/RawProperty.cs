namespace ImovelRadar.Application.Collection;

public class RawProperty
{
    public int SourceId { get; init; }

    public string? ExternalId { get; init; }

    public string? SourceUrl { get; init; }

    public string? Title { get; init; }

    public string? PropertyType { get; init; }

    public string? TransactionType { get; init; }

    public string? Price { get; init; }

    public string? Area { get; init; }

    public string? Bedrooms { get; init; }

    public string? Bathrooms { get; init; }

    public string? ParkingSpaces { get; init; }

    public string? Neighborhood { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }
}