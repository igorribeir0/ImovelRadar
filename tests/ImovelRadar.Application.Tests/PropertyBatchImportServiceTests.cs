using ImovelRadar.Application.Collection;
using ImovelRadar.Application.Collection.Enums;
using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Application.Tests;

public class PropertyBatchImportServiceTests
{
    [Fact]
    public async Task ImportAsync_ShouldProcessCompleteBatch()
    {
        var propertyRepository =
            new FakePropertyRepository();

        var historyRepository =
            new FakePriceHistoryRepository();

        var propertyImportService =
            new PropertyImportService(
                new PropertyNormalizer(),
                propertyRepository,
                historyRepository);

        var batchImportService =
            new PropertyBatchImportService(
                propertyImportService);

        var rawProperties =
            new[]
            {
                CreateRawProperty(
                    externalId: "A",
                    price: "R$ 650.000,00"),

                CreateRawProperty(
                    externalId: "B",
                    price: "R$ 700.000,00"),

                CreateRawProperty(
                    externalId: "C",
                    price: "preço inválido"),

                CreateRawProperty(
                    externalId: "A",
                    price: "R$ 650.000,00"),

                CreateRawProperty(
                    externalId: "B",
                    price: "R$ 680.000,00")
            };

        var result =
            await batchImportService.ImportAsync(
                rawProperties);

        Assert.Equal(
            5,
            result.Total);

        Assert.Equal(
            2,
            result.Created);

        Assert.Equal(
            1,
            result.Updated);

        Assert.Equal(
            1,
            result.Unchanged);

        Assert.Equal(
            1,
            result.Rejected);

        Assert.Equal(
            5,
            result.Results.Count);

        Assert.Equal(
            PropertyImportStatus.Created,
            result.Results[0].Status);

        Assert.Equal(
            PropertyImportStatus.Created,
            result.Results[1].Status);

        Assert.Equal(
            PropertyImportStatus.Rejected,
            result.Results[2].Status);

        Assert.Equal(
            PropertyImportStatus.Unchanged,
            result.Results[3].Status);

        Assert.Equal(
            PropertyImportStatus.Updated,
            result.Results[4].Status);

        Assert.Equal(
            2,
            propertyRepository.Properties.Count);

        Assert.Equal(
            3,
            historyRepository.Items.Count);
    }

    [Fact]
    public async Task ImportAsync_ShouldReturnEmptyResult_WhenBatchIsEmpty()
    {
        var propertyRepository =
            new FakePropertyRepository();

        var historyRepository =
            new FakePriceHistoryRepository();

        var propertyImportService =
            new PropertyImportService(
                new PropertyNormalizer(),
                propertyRepository,
                historyRepository);

        var batchImportService =
            new PropertyBatchImportService(
                propertyImportService);

        var result =
            await batchImportService.ImportAsync(
                Array.Empty<RawProperty>());

        Assert.Equal(
            0,
            result.Total);

        Assert.Equal(
            0,
            result.Created);

        Assert.Equal(
            0,
            result.Updated);

        Assert.Equal(
            0,
            result.Unchanged);

        Assert.Equal(
            0,
            result.Rejected);

        Assert.Empty(
            result.Results);
    }

    [Fact]
    public async Task ImportAsync_ShouldPreserveRejectedItemErrors()
    {
        var propertyRepository =
            new FakePropertyRepository();

        var historyRepository =
            new FakePriceHistoryRepository();

        var propertyImportService =
            new PropertyImportService(
                new PropertyNormalizer(),
                propertyRepository,
                historyRepository);

        var batchImportService =
            new PropertyBatchImportService(
                propertyImportService);

        var rawProperties =
            new[]
            {
                CreateRawProperty(
                    externalId: "A",
                    price: "R$ 650.000,00"),

                CreateRawProperty(
                    externalId: "B",
                    price: "preço inválido")
            };

        var result =
            await batchImportService.ImportAsync(
                rawProperties);

        Assert.Equal(
            2,
            result.Total);

        Assert.Equal(
            1,
            result.Created);

        Assert.Equal(
            1,
            result.Rejected);

        var rejectedResult =
            result.Results.Single(
                item =>
                    item.Status ==
                    PropertyImportStatus.Rejected);

        Assert.Contains(
            "O preço do imóvel é inválido.",
            rejectedResult.Errors);
    }

    [Fact]
    public async Task ImportAsync_ShouldThrow_WhenBatchIsNull()
    {
        var propertyRepository =
            new FakePropertyRepository();

        var historyRepository =
            new FakePriceHistoryRepository();

        var propertyImportService =
            new PropertyImportService(
                new PropertyNormalizer(),
                propertyRepository,
                historyRepository);

        var batchImportService =
            new PropertyBatchImportService(
                propertyImportService);

        await Assert.ThrowsAsync<ArgumentNullException>(
            async () =>
                await batchImportService.ImportAsync(
                    null!));
    }

    private static RawProperty CreateRawProperty(
        string externalId,
        string price)
    {
        return new RawProperty
        {
            SourceId = 1,
            ExternalId = externalId,
            SourceUrl =
                $"https://example.com/imovel/{externalId}",
            Title =
                $"Apartamento {externalId}",
            PropertyType =
                "Apartamento",
            TransactionType =
                "Venda",
            Price =
                price,
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

    private class FakePropertyRepository
        : IPropertyRepository
    {
        private readonly List<Property>
            _properties = [];

        public IReadOnlyList<Property> Properties =>
            _properties;

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

        public Task<Property?>
            GetBySourceAndExternalIdAsync(
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