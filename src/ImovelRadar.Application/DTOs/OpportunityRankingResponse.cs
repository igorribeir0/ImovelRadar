namespace ImovelRadar.Application.DTOs;

public class OpportunityRankingResponse
{
    public int PropertyId { get; init; }

    public string? Title { get; init; }
    public string? Neighborhood { get; init; }

    public string City { get; init; } = null!;
    public string State { get; init; } = null!;

    public decimal Price { get; init; }

    public decimal? PricePerSquareMeter { get; init; }

    public decimal MarketPricePerSquareMeter { get; init; }

    public decimal? MarketDifferencePercentage { get; init; }

    public int ComparablePropertyCount { get; init; }

    public string ConfidenceLevel { get; init; } = null!;

    public int OpportunityScore { get; init; }

    public string OpportunityLevel { get; init; } = null!;

    public bool HasPriceReduction { get; init; }

    public decimal? HistoricalVariationPercentage { get; init; }
}