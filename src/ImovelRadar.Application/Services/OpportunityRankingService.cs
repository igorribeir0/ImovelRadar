using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Application.Services;

public class OpportunityRankingService
{
    private const int MaxPageSize = 100;

    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyPriceHistoryRepository
        _priceHistoryRepository;
    private readonly PropertyComparisonService
        _propertyComparisonService;
    private readonly PropertyOpportunityAnalyzer
        _propertyOpportunityAnalyzer;
    private readonly AnalysisConfidenceService
        _analysisConfidenceService;

    public OpportunityRankingService(
        IPropertyRepository propertyRepository,
        IPropertyPriceHistoryRepository priceHistoryRepository,
        PropertyComparisonService propertyComparisonService,
        PropertyOpportunityAnalyzer propertyOpportunityAnalyzer,
        AnalysisConfidenceService analysisConfidenceService)
    {
        _propertyRepository = propertyRepository;
        _priceHistoryRepository = priceHistoryRepository;
        _propertyComparisonService = propertyComparisonService;
        _propertyOpportunityAnalyzer = propertyOpportunityAnalyzer;
        _analysisConfidenceService = analysisConfidenceService;
    }

    public async Task<PagedResponse<OpportunityRankingResponse>>
        GetRankingAsync(
            OpportunityRankingRequest request,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateRequest(request);

        var properties =
            await _propertyRepository.GetAllAsync(
                cancellationToken);

        var filteredProperties =
            ApplyPropertyFilters(
                properties,
                request);

        var ranking =
            new List<OpportunityRankingResponse>();

        foreach (var property in filteredProperties)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var candidates =
                await _propertyRepository
                    .GetComparablePropertiesAsync(
                        property,
                        cancellationToken);

            var comparables =
                _propertyComparisonService
                    .FilterComparables(
                        property,
                        candidates);

            if (comparables.Count == 0)
            {
                continue;
            }

            var marketPricePerSquareMeter =
                _propertyComparisonService
                    .CalculateMedianPricePerSquareMeter(
                        property,
                        comparables);

            if (!marketPricePerSquareMeter.HasValue)
            {
                continue;
            }

            var comparablePropertyCount =
                comparables.Count;

            var confidenceLevel =
                _analysisConfidenceService.Calculate(
                    comparablePropertyCount);

            var history =
                await _priceHistoryRepository
                    .GetByPropertyIdAsync(
                        property.Id,
                        cancellationToken);

            var analysis =
                _propertyOpportunityAnalyzer.Analyze(
                    property,
                    history,
                    marketPricePerSquareMeter.Value);

            if (request.MinScore.HasValue &&
                analysis.OpportunityScore <
                request.MinScore.Value)
            {
                continue;
            }

            ranking.Add(
                new OpportunityRankingResponse
                {
                    PropertyId = property.Id,
                    Title = property.Title,
                    Neighborhood = property.Neighborhood,
                    City = property.City,
                    State = property.State,

                    Price = analysis.PropertyPrice,

                    PricePerSquareMeter =
                        RoundNullable(
                            analysis.PricePerSquareMeter),

                    MarketPricePerSquareMeter =
                        Math.Round(
                            analysis.MarketPricePerSquareMeter,
                            2),

                    MarketDifferencePercentage =
                        RoundNullable(
                            analysis.MarketDifferencePercentage),

                    ComparablePropertyCount =
                        comparablePropertyCount,

                    ConfidenceLevel =
                        confidenceLevel.ToString(),

                    OpportunityScore =
                        analysis.OpportunityScore,

                    OpportunityLevel =
                        analysis.OpportunityLevel.ToString(),

                    HasPriceReduction =
                        analysis.HasPriceReduction,

                    HistoricalVariationPercentage =
                        RoundNullable(
                            analysis.HistoricalVariationPercentage)
                });
        }

        var orderedRanking =
            ranking
                .OrderByDescending(item =>
                    item.OpportunityScore)
                .ThenBy(item =>
                    item.MarketDifferencePercentage)
                .ThenBy(item =>
                    item.PropertyId)
                .ToList();

        var totalItems =
            orderedRanking.Count;

        var totalPages =
            totalItems == 0
                ? 0
                : (int)Math.Ceiling(
                    totalItems /
                    (double)request.PageSize);

        var items =
            orderedRanking
                .Skip(
                    (request.Page - 1) *
                    request.PageSize)
                .Take(
                    request.PageSize)
                .ToList();

        return new PagedResponse<OpportunityRankingResponse>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    private static IEnumerable<Property>
        ApplyPropertyFilters(
            IEnumerable<Property> properties,
            OpportunityRankingRequest request)
    {
        var query = properties;

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(property =>
                property.City.Equals(
                    request.City.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.State))
        {
            query = query.Where(property =>
                property.State.Equals(
                    request.State.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Neighborhood))
        {
            query = query.Where(property =>
                property.Neighborhood != null &&
                property.Neighborhood.Equals(
                    request.Neighborhood.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(property =>
                property.Price >=
                request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(property =>
                property.Price <=
                request.MaxPrice.Value);
        }

        return query;
    }

    private static void ValidateRequest(
        OpportunityRankingRequest request)
    {
        if (request.MinPrice is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.MinPrice),
                "O preço mínimo não pode ser negativo.");
        }

        if (request.MaxPrice is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.MaxPrice),
                "O preço máximo não pode ser negativo.");
        }

        if (request.MinPrice.HasValue &&
            request.MaxPrice.HasValue &&
            request.MinPrice > request.MaxPrice)
        {
            throw new ArgumentException(
                "O preço mínimo não pode ser maior que o preço máximo.");
        }

        if (request.MinScore is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.MinScore),
                "O score mínimo deve estar entre 0 e 100.");
        }

        if (request.Page < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.Page),
                "A página deve ser maior ou igual a 1.");
        }

        if (request.PageSize < 1 ||
            request.PageSize > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.PageSize),
                $"O tamanho da página deve estar entre 1 e {MaxPageSize}.");
        }
    }

    private static decimal? RoundNullable(
        decimal? value)
    {
        return value.HasValue
            ? Math.Round(value.Value, 2)
            : null;
    }
}