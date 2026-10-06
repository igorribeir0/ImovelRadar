using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Application.Collection;

public class PropertyReconciliationService
{
    private readonly IPropertyRepository
        _propertyRepository;

    public PropertyReconciliationService(
        IPropertyRepository propertyRepository)
    {
        _propertyRepository =
            propertyRepository;
    }

    public async Task<PropertyReconciliationResult> ReconcileAsync(
        CollectionRun collectionRun,
        IEnumerable<string> observedExternalIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            collectionRun);

        ArgumentNullException.ThrowIfNull(
            observedExternalIds);

        if (!collectionRun.CanReconcileMissingProperties)
        {
            throw new InvalidOperationException(
                "A coleta não permite reconciliação de imóveis ausentes.");
        }

        var scope =
            collectionRun.Scope!;

        var observedIds =
            observedExternalIds
                .Where(id =>
                    !string.IsNullOrWhiteSpace(id))
                .Select(id =>
                    id.Trim())
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

        var properties =
            await _propertyRepository.GetAllAsync(
                cancellationToken);

        var candidates =
            properties
                .Where(property =>
                    property.SourceId ==
                    collectionRun.SourceId)
                .Where(property =>
                    scope.Contains(property))
                .ToList();

        var markedAsNotObserved =
            0;

        foreach (var property in candidates)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(
                    property.ExternalId))
            {
                continue;
            }

            if (observedIds.Contains(
                    property.ExternalId))
            {
                continue;
            }

            if (property.ObservationStatus ==
                Domain.Enums.PropertyObservationStatus.NotObserved)
            {
                continue;
            }

            property.MarkAsNotObserved();

            await _propertyRepository.UpdateAsync(
                property,
                cancellationToken);

            markedAsNotObserved++;
        }

        return new PropertyReconciliationResult(
            evaluated: candidates.Count,
            markedAsNotObserved:
                markedAsNotObserved);
    }
}