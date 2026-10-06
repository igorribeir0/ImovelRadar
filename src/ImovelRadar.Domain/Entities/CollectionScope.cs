using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Entities;

public class CollectionScope
{
    public CollectionScope(
        string city,
        string state,
        PropertyType propertyType,
        TransactionType transactionType,
        string? neighborhood = null,
        int? bedrooms = null)
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

        if (bedrooms is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bedrooms),
                "A quantidade de quartos não pode ser negativa.");
        }

        City = city.Trim();
        State = state.Trim();

        Neighborhood =
            string.IsNullOrWhiteSpace(neighborhood)
                ? null
                : neighborhood.Trim();

        PropertyType = propertyType;
        TransactionType = transactionType;
        Bedrooms = bedrooms;
    }

    public string City { get; }

    public string State { get; }

    public string? Neighborhood { get; }

    public PropertyType PropertyType { get; }

    public TransactionType TransactionType { get; }

    public int? Bedrooms { get; }

    public bool Contains(Property property)
    {
        ArgumentNullException.ThrowIfNull(property);

        if (!string.Equals(
                property.City,
                City,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.Equals(
                property.State,
                State,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (property.PropertyType != PropertyType)
        {
            return false;
        }

        if (property.TransactionType != TransactionType)
        {
            return false;
        }

        if (Neighborhood is not null &&
            !string.Equals(
                property.Neighborhood,
                Neighborhood,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (Bedrooms.HasValue &&
            property.Bedrooms != Bedrooms)
        {
            return false;
        }

        return true;
    }
}