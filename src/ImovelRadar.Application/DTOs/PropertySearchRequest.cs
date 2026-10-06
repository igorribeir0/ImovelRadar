namespace ImovelRadar.Application.DTOs;

public class PropertySearchRequest
{
    public string? City { get; init; }

    public string? State { get; init; }

    public string? Neighborhood { get; init; }

    public int? Bedrooms { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public decimal? MinArea { get; init; }

    public decimal? MaxArea { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}