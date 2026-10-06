namespace ImovelRadar.Domain.Entities;

public class PropertyPriceHistory
{
    public PropertyPriceHistory(
        int propertyId,
        decimal price,
        DateTime collectedAt,
        int sourceId)
    {
        if (propertyId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(propertyId),
                "O imóvel é obrigatório.");

        if (price <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "O preço deve ser maior que zero.");

        if (sourceId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(sourceId),
                "A origem é obrigatória.");

        PropertyId = propertyId;
        Price = price;
        CollectedAt = collectedAt;
        SourceId = sourceId;
    }

    public long Id { get; private set; }

    public int PropertyId { get; private set; }

    public decimal Price { get; private set; }

    public DateTime CollectedAt { get; private set; }

    public int SourceId { get; private set; }
}