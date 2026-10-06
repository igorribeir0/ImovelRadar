using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Models;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Application.Services;

public class PropertyAnalysisService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyPriceHistoryRepository _priceHistoryRepository;
    private readonly PropertyComparisonService _propertyComparisonService;
    private readonly PropertyOpportunityAnalyzer _opportunityAnalyzer;

    public PropertyAnalysisService(
        IPropertyRepository propertyRepository,
        IPropertyPriceHistoryRepository priceHistoryRepository,
        PropertyComparisonService propertyComparisonService,
        PropertyOpportunityAnalyzer opportunityAnalyzer)
    {
        _propertyRepository = propertyRepository;
        _priceHistoryRepository = priceHistoryRepository;
        _propertyComparisonService = propertyComparisonService;
        _opportunityAnalyzer = opportunityAnalyzer;
    }

    public async Task<PropertyOpportunityAnalysis> AnalyzePropertyAsync(
        int propertyId,
        CancellationToken cancellationToken = default)
    {
        if (propertyId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(propertyId),
                "O identificador do imóvel deve ser maior que zero.");
        }

        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null)
        {
            throw new InvalidOperationException(
                "Imóvel não encontrado.");
        }

        var priceHistory =
            await _priceHistoryRepository.GetByPropertyIdAsync(
                propertyId,
                cancellationToken);

        var comparableProperties =
            await _propertyRepository.GetComparablePropertiesAsync(
                property,
                cancellationToken);

        var marketPricePerSquareMeter =
            _propertyComparisonService
                .CalculateMedianPricePerSquareMeter(
                    property,
                    comparableProperties);

        if (!marketPricePerSquareMeter.HasValue)
        {
            throw new InvalidOperationException(
                "Não existem imóveis comparáveis suficientes para calcular a referência de mercado.");
        }

        return _opportunityAnalyzer.Analyze(
            property,
            priceHistory,
            marketPricePerSquareMeter.Value);
    }
}