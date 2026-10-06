using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Application.Services;

public class PropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IPropertyPriceHistoryRepository _priceHistoryRepository;

    public PropertyService(
        IPropertyRepository propertyRepository,
        IPropertyPriceHistoryRepository priceHistoryRepository)
    {
        _propertyRepository = propertyRepository;
        _priceHistoryRepository = priceHistoryRepository;
    }

    public async Task UpdatePriceAsync(
        int propertyId,
        decimal newPrice,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(
            propertyId,
            cancellationToken);

        if (property is null)
            throw new InvalidOperationException("Imóvel não encontrado.");

        if (property.Price == newPrice)
            return;

        var priceHistory = new PropertyPriceHistory(
            propertyId: propertyId,
            price: newPrice,
            collectedAt: DateTime.UtcNow,
            sourceId: property.SourceId);

        property.UpdatePrice(newPrice);

        await _priceHistoryRepository.AddAsync(
            priceHistory,
            cancellationToken);

        await _propertyRepository.UpdateAsync(
            property,
            cancellationToken);
    }
}