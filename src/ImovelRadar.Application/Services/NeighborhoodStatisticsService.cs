using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Application.Services;

public class NeighborhoodStatisticsService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly MarketStatisticsService _marketStatisticsService;

    public NeighborhoodStatisticsService(
        IPropertyRepository propertyRepository,
        MarketStatisticsService marketStatisticsService)
    {
        _propertyRepository = propertyRepository;
        _marketStatisticsService = marketStatisticsService;
    }

    public async Task<NeighborhoodStatisticsResponse> GetAsync(
        string city,
        string state,
        string neighborhood,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException(
                "A cidade é obrigatória.",
                nameof(city));
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            throw new ArgumentException(
                "O estado é obrigatório.",
                nameof(state));
        }

        if (string.IsNullOrWhiteSpace(neighborhood))
        {
            throw new ArgumentException(
                "O bairro é obrigatório.",
                nameof(neighborhood));
        }

        var request = new PropertySearchRequest
        {
            City = city.Trim(),
            State = state.Trim(),
            Neighborhood = neighborhood.Trim()
        };

        var properties =
            await _propertyRepository.SearchAsync(
                request,
                cancellationToken);

        if (properties.Count == 0)
        {
            throw new InvalidOperationException(
                "Não existem imóveis para calcular as estatísticas do bairro.");
        }

        var prices = properties
            .Select(property => property.Price)
            .OrderBy(price => price)
            .ToList();

        return new NeighborhoodStatisticsResponse
        {
            City = city.Trim(),
            State = state.Trim(),
            Neighborhood = neighborhood.Trim(),

            PropertyCount = properties.Count,

            AveragePrice = prices.Average(),
            MedianPrice = CalculateMedian(prices),

            AveragePricePerSquareMeter =
                _marketStatisticsService
                    .CalculateAveragePricePerSquareMeter(properties),

            MedianPricePerSquareMeter =
                _marketStatisticsService
                    .CalculateMedianPricePerSquareMeter(properties),

            LowestPrice = prices.First(),
            HighestPrice = prices.Last()
        };
    }

    private static decimal CalculateMedian(
        IReadOnlyList<decimal> orderedValues)
    {
        var middle = orderedValues.Count / 2;

        if (orderedValues.Count % 2 != 0)
        {
            return orderedValues[middle];
        }

        return (
            orderedValues[middle - 1] +
            orderedValues[middle]
        ) / 2m;
    }
}