namespace ImovelRadar.Domain.Entities;

public class Location
{
    public int Id { get; private set; }

    public string? Neighborhood { get; private set; }

    public string City { get; private set; } = null!;

    public string State { get; private set; } = null!;

    public string? PostalCode { get; private set; }

    public decimal? Latitude { get; private set; }

    public decimal? Longitude { get; private set; }
}