using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Domain.Tests;

public class PropertyOpportunityAnalyzerTests
{
    [Fact]
    public void Analyze_ShouldCreateCompleteOpportunityAnalysis()
    {
        var property = CreateProperty(
            price: 650_000m,
            area: 100m);

        var history = new List<PropertyPriceHistory>
        {
            CreateHistory(
                price: 700_000m,
                date: new DateTime(2026, 1, 1)),

            CreateHistory(
                price: 680_000m,
                date: new DateTime(2026, 2, 1)),

            CreateHistory(
                price: 650_000m,
                date: new DateTime(2026, 3, 1))
        };

        var analyzer = CreateAnalyzer();

        var result = analyzer.Analyze(
            property,
            history,
            marketPricePerSquareMeter: 8_000m);

        Assert.Equal(650_000m, result.PropertyPrice);

        Assert.Equal(
            6_500m,
            result.PricePerSquareMeter);

        Assert.Equal(
            8_000m,
            result.MarketPricePerSquareMeter);

        Assert.Equal(
            -18.75m,
            result.MarketDifferencePercentage);

        Assert.Equal(
            700_000m,
            result.InitialPrice);

        Assert.NotNull(
            result.HistoricalVariationPercentage);

        Assert.Equal(
            -7.14m,
            Math.Round(
                result.HistoricalVariationPercentage.Value,
                2));

        Assert.Equal(
            2,
            result.PriceChangeCount);

        Assert.True(
            result.HasPriceReduction);

        Assert.Equal(
            90,
            result.OpportunityScore);

        Assert.Equal(
            OpportunityLevel.Excellent,
            result.OpportunityLevel);
    }

    [Fact]
    public void Analyze_ShouldReturnScoreZero_WhenPropertyHasNoArea()
    {
        var property = CreateProperty(
            price: 650_000m,
            area: null);

        var history =
            new List<PropertyPriceHistory>();

        var analyzer = CreateAnalyzer();

        var result = analyzer.Analyze(
            property,
            history,
            marketPricePerSquareMeter: 8_000m);

        Assert.Null(
            result.PricePerSquareMeter);

        Assert.Null(
            result.MarketDifferencePercentage);

        Assert.Equal(
            0,
            result.OpportunityScore);
    }

    private static PropertyOpportunityAnalyzer CreateAnalyzer()
    {
        return new PropertyOpportunityAnalyzer(
            new MarketStatisticsService(),
            new PriceHistoryAnalyzer(),
            new OpportunityScoreService());
    }

    private static Property CreateProperty(
        decimal price,
        decimal? area)
    {
        return new Property(
            title: "Apartamento em São Paulo",
            propertyType: PropertyType.Apartment,
            transactionType: TransactionType.Sale,
            price: price,
            area: area,
            bedrooms: 2,
            bathrooms: 2,
            parkingSpaces: 1,
            city: "São Paulo",
            state: "SP",
            sourceId: 1);
    }

    private static PropertyPriceHistory CreateHistory(
        decimal price,
        DateTime date)
    {
        return new PropertyPriceHistory(
            propertyId: 1,
            price: price,
            collectedAt: date,
            sourceId: 1);
    }
}