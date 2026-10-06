using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Models;

namespace ImovelRadar.Domain.Services;

public class PropertyOpportunityAnalyzer
{
    private readonly MarketStatisticsService _marketStatisticsService;
    private readonly PriceHistoryAnalyzer _priceHistoryAnalyzer;
    private readonly OpportunityScoreService _opportunityScoreService;

    public PropertyOpportunityAnalyzer(
        MarketStatisticsService marketStatisticsService,
        PriceHistoryAnalyzer priceHistoryAnalyzer,
        OpportunityScoreService opportunityScoreService)
    {
        _marketStatisticsService = marketStatisticsService;
        _priceHistoryAnalyzer = priceHistoryAnalyzer;
        _opportunityScoreService = opportunityScoreService;
    }

    public PropertyOpportunityAnalysis Analyze(
        Property property,
        IEnumerable<PropertyPriceHistory> priceHistory,
        decimal marketPricePerSquareMeter)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(priceHistory);

        var priceHistoryList =
            priceHistory.ToList();

        var pricePerSquareMeter =
            property.GetPricePerSquareMeter();

        var marketDifference =
            _marketStatisticsService
                .CalculatePercentageDifferenceFromMarket(
                    property,
                    marketPricePerSquareMeter);

        var initialPrice =
            _priceHistoryAnalyzer
                .GetInitialPrice(priceHistoryList);

        var historicalVariation =
            _priceHistoryAnalyzer
                .CalculatePriceVariationPercentage(
                    priceHistoryList);

        var priceChangeCount =
            _priceHistoryAnalyzer
                .GetPriceChangeCount(
                    priceHistoryList);

        var hasPriceReduction =
            _priceHistoryAnalyzer
                .HasPriceReduction(
                    priceHistoryList);

        var opportunityScore =
            marketDifference.HasValue
                ? _opportunityScoreService
                    .CalculateFromMarketDifference(
                        marketDifference.Value)
                : 0;

        var opportunityLevel =
            _opportunityScoreService
                .GetLevel(opportunityScore);

        return new PropertyOpportunityAnalysis
        {
            PropertyPrice = property.Price,
            PricePerSquareMeter = pricePerSquareMeter,
            MarketPricePerSquareMeter =
                marketPricePerSquareMeter,
            MarketDifferencePercentage =
                marketDifference,
            InitialPrice = initialPrice,
            HistoricalVariationPercentage =
                historicalVariation,
            PriceChangeCount = priceChangeCount,
            HasPriceReduction = hasPriceReduction,
            OpportunityScore = opportunityScore,
            OpportunityLevel = opportunityLevel
        };
    }
}