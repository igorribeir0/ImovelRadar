using ImovelRadar.Application.Collection;
using ImovelRadar.Application.Collection.Enums;
using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Application.Tests;

public class PropertyImportServiceTests
{
    [Fact]
public async Task ImportAsync_ShouldPreserveCompletePriceHistoryAcrossImports()
{
    var propertyRepository =
        new FakePropertyRepository();

    var historyRepository =
        new FakePriceHistoryRepository();

    var service =
        CreateService(
            propertyRepository,
            historyRepository);

    var firstCollection =
        CreateRawProperty(
            sourceId: 1,
            externalId: "ABC-123",
            price: "R$ 650.000,00");

    var secondCollection =
        CreateRawProperty(
            sourceId: 1,
            externalId: "ABC-123",
            price: "R$ 650.000,00");

    var thirdCollection =
        CreateRawProperty(
            sourceId: 1,
            externalId: "ABC-123",
            price: "R$ 620.000,00");

    var firstResult =
        await service.ImportAsync(
            firstCollection);

    var secondResult =
        await service.ImportAsync(
            secondCollection);

    var thirdResult =
        await service.ImportAsync(
            thirdCollection);

    Assert.Equal(
        PropertyImportStatus.Created,
        firstResult.Status);

    Assert.Equal(
        PropertyImportStatus.Unchanged,
        secondResult.Status);

    Assert.Equal(
        PropertyImportStatus.Updated,
        thirdResult.Status);

    Assert.Single(
        propertyRepository.Properties);

    var property =
        propertyRepository.Properties[0];

    Assert.Equal(
        620_000m,
        property.Price);

    Assert.Equal(
        2,
        historyRepository.Items.Count);

    Assert.Equal(
        650_000m,
        historyRepository.Items[0].Price);

    Assert.Equal(
        620_000m,
        historyRepository.Items[1].Price);

    Assert.Equal(
        property.Id,
        historyRepository.Items[0].PropertyId);

    Assert.Equal(
        property.Id,
        historyRepository.Items[1].PropertyId);
}

    [Fact]
    public async Task ImportAsync_ShouldReturnUnchanged_WhenPriceHasNotChanged()
    {
        var existingProperty =
            CreateExistingProperty(
                id: 10,
                sourceId: 1,
                externalId: "ABC-123",
                price: 650_000m);

        var propertyRepository =
            new FakePropertyRepository(
                existingProperty);

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var rawProperty =
            CreateRawProperty(
                sourceId: 1,
                externalId: "ABC-123",
                price: "R$ 650.000,00");

        var result =
            await service.ImportAsync(rawProperty);

        Assert.True(result.IsSuccess);

        Assert.Equal(
            PropertyImportStatus.Unchanged,
            result.Status);

        Assert.Same(
            existingProperty,
            result.Property);

        Assert.Equal(
            650_000m,
            existingProperty.Price);

        Assert.Empty(
            historyRepository.Items);

        Assert.Equal(
            1,
            propertyRepository.UpdateCount);
    }

    [Fact]
public async Task ImportAsync_ShouldUpdatePropertyAndCreateHistory_WhenPriceChanges()
{
    var existingProperty =
        CreateExistingProperty(
            id: 10,
            sourceId: 1,
            externalId: "ABC-123",
            price: 650_000m);

    var previousLastSeenAt =
        existingProperty.LastSeenAt;

    var propertyRepository =
        new FakePropertyRepository(
            existingProperty);

    var historyRepository =
        new FakePriceHistoryRepository();

    var service =
        CreateService(
            propertyRepository,
            historyRepository);

    var rawProperty =
        CreateRawProperty(
            sourceId: 1,
            externalId: "ABC-123",
            price: "R$ 620.000,00");

    var result =
        await service.ImportAsync(rawProperty);

    Assert.True(result.IsSuccess);

    Assert.Equal(
        PropertyImportStatus.Updated,
        result.Status);

    Assert.Equal(
        620_000m,
        existingProperty.Price);

    Assert.Equal(
        1,
        propertyRepository.UpdateCount);

    Assert.True(
        existingProperty.LastSeenAt >=
        previousLastSeenAt);

    var history =
        Assert.Single(
            historyRepository.Items);

    Assert.Equal(
        10,
        history.PropertyId);

    Assert.Equal(
        620_000m,
        history.Price);

    Assert.Equal(
        1,
        history.SourceId);
}

    [Fact]
    public async Task ImportAsync_ShouldRejectInvalidProperty()
    {
        var propertyRepository =
            new FakePropertyRepository();

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var rawProperty =
            CreateRawProperty(
                sourceId: 1,
                externalId: "ABC-123",
                price: "preço sob consulta");

        var result =
            await service.ImportAsync(rawProperty);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            PropertyImportStatus.Rejected,
            result.Status);

        Assert.Null(result.Property);

        Assert.Contains(
            "O preço do imóvel é inválido.",
            result.Errors);

        Assert.Empty(
            propertyRepository.Properties);

        Assert.Empty(
            historyRepository.Items);
    }

    [Fact]
    public async Task ImportAsync_ShouldRejectPropertyWithoutExternalId()
    {
        var propertyRepository =
            new FakePropertyRepository();

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var rawProperty =
            CreateRawProperty(
                sourceId: 1,
                externalId: null,
                price: "R$ 650.000,00");

        var result =
            await service.ImportAsync(rawProperty);

        Assert.False(result.IsSuccess);

        Assert.Equal(
            PropertyImportStatus.Rejected,
            result.Status);

        Assert.Contains(
            "O identificador externo é obrigatório para importação.",
            result.Errors);

        Assert.Empty(
            propertyRepository.Properties);
    }

    [Fact]
    public async Task ImportAsync_ShouldNotTreatSameExternalIdFromDifferentSourceAsDuplicate()
    {
        var existingProperty =
            CreateExistingProperty(
                id: 10,
                sourceId: 1,
                externalId: "ABC-123",
                price: 650_000m);

        var propertyRepository =
            new FakePropertyRepository(
                existingProperty);

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var rawProperty =
            CreateRawProperty(
                sourceId: 2,
                externalId: "ABC-123",
                price: "R$ 700.000,00");

        var result =
            await service.ImportAsync(rawProperty);

        Assert.Equal(
            PropertyImportStatus.Created,
            result.Status);

        Assert.Equal(
            2,
            propertyRepository.Properties.Count);

        Assert.Equal(
            1,
            propertyRepository.Properties[0].SourceId);

        Assert.Equal(
            2,
            propertyRepository.Properties[1].SourceId);

        Assert.Equal(
            "ABC-123",
            propertyRepository.Properties[0].ExternalId);

        Assert.Equal(
            "ABC-123",
            propertyRepository.Properties[1].ExternalId);
    }

    private static PropertyImportService CreateService(
        FakePropertyRepository propertyRepository,
        FakePriceHistoryRepository historyRepository)
    {
        return new PropertyImportService(
            new PropertyNormalizer(),
            propertyRepository,
            historyRepository);
    }

    private static RawProperty CreateRawProperty(
        int sourceId,
        string? externalId,
        string price)
    {
        return new RawProperty
        {
            SourceId = sourceId,
            ExternalId = externalId,
            SourceUrl =
                "https://example.com/imovel/ABC-123",
            Title =
                "Apartamento no Tatuapé",
            PropertyType =
                "Apartamento",
            TransactionType =
                "Venda",
            Price = price,
            Area =
                "100 m²",
            Bedrooms =
                "2 quartos",
            Bathrooms =
                "2 banheiros",
            ParkingSpaces =
                "1 vaga",
            Neighborhood =
                "Tatuapé",
            City =
                "São Paulo",
            State =
                "SP"
        };
    }

    private static Property CreateExistingProperty(
        int id,
        int sourceId,
        string externalId,
        decimal price)
    {
        var property =
            new Property(
                title:
                    "Apartamento no Tatuapé",
                propertyType:
                    PropertyType.Apartment,
                transactionType:
                    TransactionType.Sale,
                price:
                    price,
                area:
                    100m,
                bedrooms:
                    2,
                bathrooms:
                    2,
                parkingSpaces:
                    1,
                city:
                    "São Paulo",
                state:
                    "SP",
                sourceId:
                    sourceId,
                neighborhood:
                    "Tatuapé",
                externalId:
                    externalId,
                sourceUrl:
                    "https://example.com/imovel/ABC-123");

        property.AssignId(id);

        return property;
    }

    private class FakePropertyRepository
        : IPropertyRepository
    {
        private readonly List<Property>
            _properties = [];

        public FakePropertyRepository()
        {
        }

        public FakePropertyRepository(
            params Property[] properties)
        {
            _properties.AddRange(properties);
        }

        public IReadOnlyList<Property> Properties =>
            _properties;

        public int UpdateCount { get; private set; }

        public Task<IReadOnlyList<Property>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Property> result =
                _properties.ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<Property>> SearchAsync(
            PropertySearchRequest request,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Property> result =
                _properties.ToList();

            return Task.FromResult(result);
        }

        public Task<Property?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var property =
                _properties.FirstOrDefault(
                    item => item.Id == id);

            return Task.FromResult(property);
        }

        public Task<Property?> GetBySourceAndExternalIdAsync(
            int sourceId,
            string externalId,
            CancellationToken cancellationToken = default)
        {
            var property =
                _properties.FirstOrDefault(
                    item =>
                        item.SourceId == sourceId &&
                        string.Equals(
                            item.ExternalId,
                            externalId,
                            StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(property);
        }

        public Task<IReadOnlyList<Property>>
            GetComparablePropertiesAsync(
                Property property,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Property> result =
                Array.Empty<Property>();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            Property property,
            CancellationToken cancellationToken = default)
        {
            if (property.Id == 0)
            {
                var nextId =
                    _properties.Count == 0
                        ? 1
                        : _properties.Max(
                            item => item.Id) + 1;

                property.AssignId(nextId);
            }

            _properties.Add(property);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            Property property,
            CancellationToken cancellationToken = default)
        {
            UpdateCount++;

            return Task.CompletedTask;
        }
    }

    private class FakePriceHistoryRepository
        : IPropertyPriceHistoryRepository
    {
        private readonly List<PropertyPriceHistory>
            _items = [];

        public IReadOnlyList<PropertyPriceHistory> Items =>
            _items;

        public Task<IReadOnlyList<PropertyPriceHistory>>
            GetByPropertyIdAsync(
                int propertyId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<PropertyPriceHistory> result =
                _items
                    .Where(item =>
                        item.PropertyId == propertyId)
                    .ToList();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            PropertyPriceHistory priceHistory,
            CancellationToken cancellationToken = default)
        {
            _items.Add(priceHistory);

            return Task.CompletedTask;
        }
    }
}