using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Domain.Services;

public class MarketStatisticsService
{
    public decimal? CalculateAveragePricePerSquareMeter(IEnumerable<Property> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        var pricesPerSquareMeter = properties
            .Select(property => property.GetPricePerSquareMeter())
            .Where(price => price.HasValue)
            .Select(price => price!.Value)
            .ToList();

        if (pricesPerSquareMeter.Count == 0)
            return null;

        return pricesPerSquareMeter.Average();
    }


public decimal? CalculateMedianPricePerSquareMeter(IEnumerable<Property> properties)
{
    ArgumentNullException.ThrowIfNull(properties);

    var pricesPerSquareMeter = properties
        .Select(property => property.GetPricePerSquareMeter())
        .Where(price => price.HasValue)
        .Select(price => price!.Value)
        .OrderBy(price => price)
        .ToList();

    if (pricesPerSquareMeter.Count == 0)
        return null;

    var middle = pricesPerSquareMeter.Count / 2;

    if (pricesPerSquareMeter.Count % 2 != 0)
        return pricesPerSquareMeter[middle];

    return (
        pricesPerSquareMeter[middle - 1] +
        pricesPerSquareMeter[middle]
    ) / 2;
}


public decimal? CalculatePercentageDifferenceFromMarket(
    Property property,
    decimal marketPricePerSquareMeter)
{
    ArgumentNullException.ThrowIfNull(property);

    if (marketPricePerSquareMeter <= 0)
        throw new ArgumentOutOfRangeException(
            nameof(marketPricePerSquareMeter),
            "O preço por m² de referência deve ser maior que zero.");

    var propertyPricePerSquareMeter =
        property.GetPricePerSquareMeter();

    if (!propertyPricePerSquareMeter.HasValue)
        return null;

    return (
        (propertyPricePerSquareMeter.Value - marketPricePerSquareMeter)
        / marketPricePerSquareMeter
    ) * 100;
}

}