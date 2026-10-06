using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Domain.Services;

public class PropertyComparisonService
{
    private readonly MarketStatisticsService _marketStatisticsService;

    public PropertyComparisonService(
        MarketStatisticsService marketStatisticsService)
    {
        _marketStatisticsService = marketStatisticsService;
    }

    // É ESTE MÉTODO AQUI
    public bool IsComparable(
        Property referenceProperty,
        Property candidate)
    {
        ArgumentNullException.ThrowIfNull(referenceProperty);
        ArgumentNullException.ThrowIfNull(candidate);

        if (referenceProperty.PropertyType != candidate.PropertyType)
            return false;

        if (referenceProperty.TransactionType != candidate.TransactionType)
            return false;

        if (!string.Equals(
                referenceProperty.City,
                candidate.City,
                StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.Equals(
                referenceProperty.State,
                candidate.State,
                StringComparison.OrdinalIgnoreCase))
            return false;

        if (string.IsNullOrWhiteSpace(referenceProperty.Neighborhood) ||
            string.IsNullOrWhiteSpace(candidate.Neighborhood))
        {
            return false;
        }

        if (!string.Equals(
                referenceProperty.Neighborhood.Trim(),
                candidate.Neighborhood.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (referenceProperty.Bedrooms != candidate.Bedrooms)
            return false;

        return true;
    }

    public IReadOnlyList<Property> FilterComparables(
        Property referenceProperty,
        IEnumerable<Property> candidates)
    {
        ArgumentNullException.ThrowIfNull(referenceProperty);
        ArgumentNullException.ThrowIfNull(candidates);

        return candidates
            .Where(candidate =>
                !ReferenceEquals(candidate, referenceProperty))
            .Where(candidate =>
                IsComparable(referenceProperty, candidate))
            .ToList();
    }

    public decimal? CalculateAveragePricePerSquareMeter(
        Property referenceProperty,
        IEnumerable<Property> candidates)
    {
        var comparables = FilterComparables(
            referenceProperty,
            candidates);

        return _marketStatisticsService
            .CalculateAveragePricePerSquareMeter(comparables);
    }

    public decimal? CalculateMedianPricePerSquareMeter(
        Property referenceProperty,
        IEnumerable<Property> candidates)
    {
        var comparables = FilterComparables(
            referenceProperty,
            candidates);

        return _marketStatisticsService
            .CalculateMedianPricePerSquareMeter(comparables);
    }
}