using ImovelRadar.Application.Interfaces;
using ImovelRadar.Application.Services;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;
using ImovelRadar.Application.DTOs;

namespace ImovelRadar.Application.Tests;

public class PropertyServiceTests
{


    [Fact]
public async Task UpdatePriceAsync_ShouldUpdatePriceAndCreateHistory()
{
    // Arrange
    var property = CreateValidProperty();

    var propertyRepository = new FakePropertyRepository
    {
        Property = property
    };

    var historyRepository = new FakePropertyPriceHistoryRepository();

    var service = new PropertyService(
        propertyRepository,
        historyRepository);

    // Act
    await service.UpdatePriceAsync(
        propertyId: 1,
        newPrice: 570_000m);

    // Assert
    Assert.Equal(570_000m, property.Price);
    Assert.True(propertyRepository.UpdateCalled);

    Assert.Single(historyRepository.Items);

    var history = historyRepository.Items[0];

    Assert.Equal(570_000m, history.Price);
    Assert.Equal(1, history.SourceId);
}


[Fact]
public async Task UpdatePriceAsync_ShouldNotUpdateOrCreateHistory_WhenPriceHasNotChanged()
{
    // Arrange
    var property = CreateValidProperty();

    var propertyRepository = new FakePropertyRepository
    {
        Property = property
    };

    var historyRepository = new FakePropertyPriceHistoryRepository();

    var service = new PropertyService(
        propertyRepository,
        historyRepository);

    // Act
    await service.UpdatePriceAsync(
        propertyId: 1,
        newPrice: 600_000m);

    // Assert
    Assert.Equal(600_000m, property.Price);
    Assert.False(propertyRepository.UpdateCalled);
    Assert.Empty(historyRepository.Items);
}


[Fact]
public async Task UpdatePriceAsync_ShouldThrowException_WhenPropertyDoesNotExist()
{
    // Arrange
    var propertyRepository = new FakePropertyRepository
    {
        Property = null
    };

    var historyRepository = new FakePropertyPriceHistoryRepository();

    var service = new PropertyService(
        propertyRepository,
        historyRepository);

    // Act
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
        service.UpdatePriceAsync(
            propertyId: 999,
            newPrice: 570_000m));

    // Assert
    Assert.Equal("Imóvel não encontrado.", exception.Message);

    Assert.False(propertyRepository.UpdateCalled);
    Assert.Empty(historyRepository.Items);
}

private static Property CreateValidProperty(
    decimal price = 600_000m,
    decimal? area = 75m,
    int sourceId = 1)
{
    return new Property(
        title: "Apartamento em São Paulo",
        propertyType: PropertyType.Apartment,
        transactionType: TransactionType.Sale,
        price: price,
        area: area,
        bedrooms: 2,
        bathrooms: 2,
        parkingSpaces: 1,
        city: "São Paulo",
        state: "SP",
        sourceId: sourceId);
    }

private class FakePropertyRepository : IPropertyRepository
{
    public Property? Property { get; set; }
    public bool UpdateCalled { get; private set; }

    public Task<Property?> GetBySourceAndExternalIdAsync(
    int sourceId,
    string externalId,
    CancellationToken cancellationToken = default)
{
    return Task.FromResult<Property?>(
        null);
}

public Task AddAsync(
    Property property,
    CancellationToken cancellationToken = default)
{
    return Task.CompletedTask;
}

    public Task<IReadOnlyList<Property>> SearchAsync(
        PropertySearchRequest request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Property> properties =
            Property is null
                ? Array.Empty<Property>()
                : new[] { Property };

        return Task.FromResult(properties);
    }

    public Task<IReadOnlyList<Property>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Property> properties =
            Property is null
                ? Array.Empty<Property>()
                : new[] { Property };

        return Task.FromResult(properties);
    }

    public Task<Property?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Property);
    }

    public Task<IReadOnlyList<Property>> GetComparablePropertiesAsync(
        Property property,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Property> properties =
            Array.Empty<Property>();

        return Task.FromResult(properties);
    }



    public Task UpdateAsync(
        Property property,
        CancellationToken cancellationToken = default)
    {
        Property = property;
        UpdateCalled = true;

        return Task.CompletedTask;
    }
}

private class FakePropertyPriceHistoryRepository
    : IPropertyPriceHistoryRepository{
    public List<PropertyPriceHistory> Items { get; } = [];

    public Task<IReadOnlyList<PropertyPriceHistory>> GetByPropertyIdAsync(
        int propertyId,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PropertyPriceHistory> history = Items;

        return Task.FromResult(history);
    }

    public Task AddAsync(
        PropertyPriceHistory priceHistory,
        CancellationToken cancellationToken = default)
    {
        Items.Add(priceHistory);

        return Task.CompletedTask;
    }
}
}