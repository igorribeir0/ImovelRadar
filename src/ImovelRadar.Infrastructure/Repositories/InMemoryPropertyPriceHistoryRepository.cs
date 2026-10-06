using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Infrastructure.Repositories;

public class InMemoryPropertyPriceHistoryRepository
    : IPropertyPriceHistoryRepository
{
    private readonly List<PropertyPriceHistory> _history =
    [
        new PropertyPriceHistory(
            propertyId: 1,
            price: 700_000m,
            collectedAt: new DateTime(2026, 1, 1),
            sourceId: 1),

        new PropertyPriceHistory(
            propertyId: 1,
            price: 680_000m,
            collectedAt: new DateTime(2026, 2, 1),
            sourceId: 1),

        new PropertyPriceHistory(
            propertyId: 1,
            price: 650_000m,
            collectedAt: new DateTime(2026, 3, 1),
            sourceId: 1)
    ];

    public Task<IReadOnlyList<PropertyPriceHistory>> GetByPropertyIdAsync(
        int propertyId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PropertyPriceHistory> history = _history
            .Where(item => item.PropertyId == propertyId)
            .OrderBy(item => item.CollectedAt)
            .ToList();

        return Task.FromResult(history);
    }

    public Task AddAsync(
        PropertyPriceHistory priceHistory,
        CancellationToken cancellationToken = default)
    {
        _history.Add(priceHistory);

        return Task.CompletedTask;
    }
}