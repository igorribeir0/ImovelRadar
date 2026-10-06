using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Application.Interfaces;

public interface IPropertyPriceHistoryRepository
{
    Task<IReadOnlyList<PropertyPriceHistory>> GetByPropertyIdAsync(
        int propertyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PropertyPriceHistory priceHistory,
        CancellationToken cancellationToken = default);
}