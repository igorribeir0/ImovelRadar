using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Domain.Services;

public class PriceHistoryAnalyzer
{
    public decimal? CalculatePriceVariationPercentage(
        IEnumerable<PropertyPriceHistory> history)
    {
        var orderedHistory = GetOrderedHistory(history);

        if (orderedHistory.Count < 2)
            return null;

        var initialPrice = orderedHistory.First().Price;
        var currentPrice = orderedHistory.Last().Price;

        return ((currentPrice - initialPrice) / initialPrice) * 100;
    }

    public bool HasPriceReduction(
        IEnumerable<PropertyPriceHistory> history)
    {
        var variation = CalculatePriceVariationPercentage(history);

        return variation.HasValue && variation.Value < 0;
    }

    public decimal? GetLowestPrice(
        IEnumerable<PropertyPriceHistory> history)
    {
        var historyList = GetOrderedHistory(history);

        if (historyList.Count == 0)
            return null;

        return historyList.Min(item => item.Price);
    }

    public decimal? GetHighestPrice(
        IEnumerable<PropertyPriceHistory> history)
    {
        var historyList = GetOrderedHistory(history);

        if (historyList.Count == 0)
            return null;

        return historyList.Max(item => item.Price);
    }

    public decimal? GetInitialPrice(
        IEnumerable<PropertyPriceHistory> history)
    {
        var historyList = GetOrderedHistory(history);

        if (historyList.Count == 0)
            return null;

        return historyList.First().Price;
    }

    public decimal? GetCurrentPrice(
        IEnumerable<PropertyPriceHistory> history)
    {
        var historyList = GetOrderedHistory(history);

        if (historyList.Count == 0)
            return null;

        return historyList.Last().Price;
    }

    public int GetPriceChangeCount(
        IEnumerable<PropertyPriceHistory> history)
    {
        var historyList = GetOrderedHistory(history);

        if (historyList.Count < 2)
            return 0;

        var changes = 0;

        for (var index = 1; index < historyList.Count; index++)
        {
            if (historyList[index].Price != historyList[index - 1].Price)
                changes++;
        }

        return changes;
    }

    private static List<PropertyPriceHistory> GetOrderedHistory(
        IEnumerable<PropertyPriceHistory> history)
    {
        ArgumentNullException.ThrowIfNull(history);

        return history
            .OrderBy(item => item.CollectedAt)
            .ToList();
    }
}