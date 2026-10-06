using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Models;

public class PropertyOpportunityAnalysis
{
    public decimal PropertyPrice { get; init; }

    public decimal? PricePerSquareMeter { get; init; }

    public decimal MarketPricePerSquareMeter { get; init; }

    public decimal? MarketDifferencePercentage { get; init; }

    public decimal? InitialPrice { get; init; }

    public decimal? HistoricalVariationPercentage { get; init; }

    public int PriceChangeCount { get; init; }

    public bool HasPriceReduction { get; init; }

    public int OpportunityScore { get; init; }

    public OpportunityLevel OpportunityLevel { get; init; }
}