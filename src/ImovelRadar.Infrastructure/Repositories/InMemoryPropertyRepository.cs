using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Infrastructure.Repositories;

public class InMemoryPropertyRepository : IPropertyRepository
{
    private readonly List<Property> _properties;

    public InMemoryPropertyRepository()
    {
        _properties =
        [
            CreateProperty(
                id: 1,
                price: 650_000m,
                area: 100m,
                neighborhood: "Tatuapé"),

            CreateProperty(
                id: 2,
                price: 640_000m,
                area: 80m,
                neighborhood: "Tatuapé"),

            CreateProperty(
                id: 3,
                price: 900_000m,
                area: 90m,
                neighborhood: "Tatuapé"),

            CreateProperty(
                id: 4,
                price: 5_000_000m,
                area: 100m,
                neighborhood: "Mooca")
        ];
    }

    public Task<IReadOnlyList<Property>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Property> properties =
            _properties.ToList();

        return Task.FromResult(properties);
    }

    public Task<IReadOnlyList<Property>> SearchAsync(
        PropertySearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IEnumerable<Property> query =
            _properties;

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(property =>
                property.City.Equals(
                    request.City,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.State))
        {
            query = query.Where(property =>
                property.State.Equals(
                    request.State,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Neighborhood))
        {
            query = query.Where(property =>
                property.Neighborhood != null &&
                property.Neighborhood.Equals(
                    request.Neighborhood,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (request.Bedrooms.HasValue)
        {
            query = query.Where(property =>
                property.Bedrooms ==
                request.Bedrooms.Value);
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

        if (request.MinArea.HasValue)
        {
            query = query.Where(property =>
                property.Area.HasValue &&
                property.Area.Value >=
                request.MinArea.Value);
        }

        if (request.MaxArea.HasValue)
        {
            query = query.Where(property =>
                property.Area.HasValue &&
                property.Area.Value <=
                request.MaxArea.Value);
        }

        IReadOnlyList<Property> result =
            query.ToList();

        return Task.FromResult(result);
    }

    public Task<Property?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var property =
            _properties.FirstOrDefault(
                property =>
                    property.Id == id);

        return Task.FromResult(property);
    }

    public Task<Property?> GetBySourceAndExternalIdAsync(
        int sourceId,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var property =
            _properties.FirstOrDefault(
                property =>
                    property.SourceId == sourceId &&
                    string.Equals(
                        property.ExternalId,
                        externalId,
                        StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(property);
    }

    public Task<IReadOnlyList<Property>>
        GetComparablePropertiesAsync(
            Property property,
            CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Property> properties =
            _properties
                .Where(candidate =>
                    !ReferenceEquals(
                        candidate,
                        property))
                .ToList();

        return Task.FromResult(properties);
    }

    public Task AddAsync(
        Property property,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(property);

        if (property.Id == 0)
        {
            var nextId =
                _properties.Count == 0
                    ? 1
                    : _properties.Max(
                        existingProperty =>
                            existingProperty.Id) + 1;

            property.AssignId(nextId);
        }

        _properties.Add(property);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Property property,
        CancellationToken cancellationToken = default)
    {
        // Nesta implementação temporária, o objeto já está
        // armazenado por referência na lista.
        return Task.CompletedTask;
    }

    private static Property CreateProperty(
        int id,
        decimal price,
        decimal area,
        string neighborhood)
    {
        var property =
            new Property(
                title: "Apartamento em São Paulo",
                propertyType:
                    PropertyType.Apartment,
                transactionType:
                    TransactionType.Sale,
                price: price,
                area: area,
                bedrooms: 2,
                bathrooms: 2,
                parkingSpaces: 1,
                city: "São Paulo",
                state: "SP",
                sourceId: 1,
                neighborhood: neighborhood);

        property.AssignId(id);

        return property;
    }
}