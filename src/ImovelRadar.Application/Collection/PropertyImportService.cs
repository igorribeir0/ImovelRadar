using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Application.Collection;

public class PropertyImportService
{
    private readonly PropertyNormalizer _propertyNormalizer;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyPriceHistoryRepository
        _priceHistoryRepository;

    public PropertyImportService(
        PropertyNormalizer propertyNormalizer,
        IPropertyRepository propertyRepository,
        IPropertyPriceHistoryRepository priceHistoryRepository)
    {
        _propertyNormalizer =
            propertyNormalizer;

        _propertyRepository =
            propertyRepository;

        _priceHistoryRepository =
            priceHistoryRepository;
    }

    public async Task<PropertyImportResult> ImportAsync(
        RawProperty rawProperty,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rawProperty);

        var normalizationResult =
            _propertyNormalizer.Normalize(
                rawProperty);

        if (!normalizationResult.IsSuccess)
        {
            return PropertyImportResult.Rejected(
                normalizationResult.Errors);
        }

        var normalizedProperty =
            normalizationResult.Property!;

        if (string.IsNullOrWhiteSpace(
                normalizedProperty.ExternalId))
        {
            return PropertyImportResult.Rejected(
                new[]
                {
                    "O identificador externo é obrigatório para importação."
                });
        }

        var existingProperty =
            await _propertyRepository
                .GetBySourceAndExternalIdAsync(
                    normalizedProperty.SourceId,
                    normalizedProperty.ExternalId,
                    cancellationToken);

        // Imóvel ainda não existe.
        if (existingProperty is null)
        {
            await _propertyRepository.AddAsync(
                normalizedProperty,
                cancellationToken);

            var initialPriceHistory =
                new PropertyPriceHistory(
                    propertyId:
                        normalizedProperty.Id,

                    price:
                        normalizedProperty.Price,

                    collectedAt:
                        DateTime.UtcNow,

                    sourceId:
                        normalizedProperty.SourceId);

            await _priceHistoryRepository.AddAsync(
                initialPriceHistory,
                cancellationToken);

            return PropertyImportResult.Created(
                normalizedProperty);
        }

        // O anúncio apareceu novamente na fonte.
        // Atualizamos LastSeenAt mesmo que o preço
        // continue exatamente igual.
        var seenAt =
            DateTime.UtcNow;

        existingProperty.MarkAsSeen(
            seenAt);

        // Mesmo preço: não criamos outro histórico.
        // Porém persistimos a alteração de LastSeenAt.
        if (existingProperty.Price ==
            normalizedProperty.Price)
        {
            await _propertyRepository.UpdateAsync(
                existingProperty,
                cancellationToken);

            return PropertyImportResult.Unchanged(
                existingProperty);
        }

        // O anúncio já existia e o preço mudou.
        existingProperty.UpdatePrice(
            normalizedProperty.Price);

        var priceHistory =
            new PropertyPriceHistory(
                propertyId:
                    existingProperty.Id,

                price:
                    normalizedProperty.Price,

                collectedAt:
                    seenAt,

                sourceId:
                    normalizedProperty.SourceId);

        await _priceHistoryRepository.AddAsync(
            priceHistory,
            cancellationToken);

        await _propertyRepository.UpdateAsync(
            existingProperty,
            cancellationToken);

        return PropertyImportResult.Updated(
            existingProperty);
    }
}