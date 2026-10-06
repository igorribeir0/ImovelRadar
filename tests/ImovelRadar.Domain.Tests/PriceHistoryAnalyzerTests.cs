using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Domain.Tests;

public class PriceHistoryAnalyzerTests
{
    private readonly PriceHistoryAnalyzer _analyzer = new();

    [Fact]
    public void CalculatePriceVariationPercentage_ShouldCalculateReduction()
    {
        var history = CreateHistory(
            (700_000m, new DateTime(2026, 1, 1)),
            (650_000m, new DateTime(2026, 2, 1)));

        var variation =
            _analyzer.CalculatePriceVariationPercentage(history);

        Assert.NotNull(variation);
        Assert.Equal(
            -7.14m,
            Math.Round(variation.Value, 2));
    }

    [Fact]
    public void CalculatePriceVariationPercentage_ShouldCalculateIncrease()
    {
        var history = CreateHistory(
            (600_000m, new DateTime(2026, 1, 1)),
            (660_000m, new DateTime(2026, 2, 1)));

        var variation =
            _analyzer.CalculatePriceVariationPercentage(history);

        Assert.Equal(10m, variation);
    }

    [Fact]
    public void CalculatePriceVariationPercentage_ShouldReturnNull_WhenThereIsOnlyOneRecord()
    {
        var history = CreateHistory(
            (600_000m, new DateTime(2026, 1, 1)));

        var variation =
            _analyzer.CalculatePriceVariationPercentage(history);

        Assert.Null(variation);
    }

    [Fact]
    public void HasPriceReduction_ShouldReturnTrue_WhenPriceWasReduced()
    {
        var history = CreateHistory(
            (700_000m, new DateTime(2026, 1, 1)),
            (650_000m, new DateTime(2026, 2, 1)));

        var result = _analyzer.HasPriceReduction(history);

        Assert.True(result);
    }

    [Fact]
    public void HasPriceReduction_ShouldReturnFalse_WhenPriceIncreased()
    {
        var history = CreateHistory(
            (600_000m, new DateTime(2026, 1, 1)),
            (650_000m, new DateTime(2026, 2, 1)));

        var result = _analyzer.HasPriceReduction(history);

        Assert.False(result);
    }

    [Fact]
    public void GetLowestPrice_ShouldReturnLowestPrice()
    {
        var history = CreateHistory(
            (700_000m, new DateTime(2026, 1, 1)),
            (680_000m, new DateTime(2026, 2, 1)),
            (650_000m, new DateTime(2026, 3, 1)));

        var lowestPrice = _analyzer.GetLowestPrice(history);

        Assert.Equal(650_000m, lowestPrice);
    }

    [Fact]
    public void GetHighestPrice_ShouldReturnHighestPrice()
    {
        var history = CreateHistory(
            (700_000m, new DateTime(2026, 1, 1)),
            (720_000m, new DateTime(2026, 2, 1)),
            (650_000m, new DateTime(2026, 3, 1)));

        var highestPrice = _analyzer.GetHighestPrice(history);

        Assert.Equal(720_000m, highestPrice);
    }

    [Fact]
    public void GetInitialAndCurrentPrice_ShouldUseCollectionDate()
    {
        // Intencionalmente fora de ordem.
        var history = CreateHistory(
            (650_000m, new DateTime(2026, 3, 1)),
            (700_000m, new DateTime(2026, 1, 1)),
            (680_000m, new DateTime(2026, 2, 1)));

        var initialPrice = _analyzer.GetInitialPrice(history);
        var currentPrice = _analyzer.GetCurrentPrice(history);

        Assert.Equal(700_000m, initialPrice);
        Assert.Equal(650_000m, currentPrice);
    }

    [Fact]
    public void GetPriceChangeCount_ShouldCountOnlyActualChanges()
    {
        var history = CreateHistory(
            (700_000m, new DateTime(2026, 1, 1)),
            (680_000m, new DateTime(2026, 2, 1)),
            (680_000m, new DateTime(2026, 3, 1)),
            (650_000m, new DateTime(2026, 4, 1)));

        var changes =
            _analyzer.GetPriceChangeCount(history);

        Assert.Equal(2, changes);
    }

    [Fact]
    public void EmptyHistory_ShouldReturnNullOrZero()
    {
        var history = new List<PropertyPriceHistory>();

        Assert.Null(_analyzer.GetInitialPrice(history));
        Assert.Null(_analyzer.GetCurrentPrice(history));
        Assert.Null(_analyzer.GetLowestPrice(history));
        Assert.Null(_analyzer.GetHighestPrice(history));
        Assert.Null(
            _analyzer.CalculatePriceVariationPercentage(history));

        Assert.False(_analyzer.HasPriceReduction(history));
        Assert.Equal(
            0,
            _analyzer.GetPriceChangeCount(history));
    }

    private static List<PropertyPriceHistory> CreateHistory(
        params (decimal Price, DateTime CollectedAt)[] items)
    {
        return items
            .Select(item => new PropertyPriceHistory(
                propertyId: 1,
                price: item.Price,
                collectedAt: item.CollectedAt,
                sourceId: 1))
            .ToList();
    }
}