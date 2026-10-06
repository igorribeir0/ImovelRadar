using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Application.Services;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Application.Tests;

public class OpportunityRankingServiceTests
{
    [Fact]
    public async Task GetRankingAsync_ShouldOrderPropertiesByOpportunityScore()
    {
        var property1 = CreateProperty(
            id: 1,
            price: 650_000m,
            area: 100m,
            neighborhood: "Tatuapé");

        var property2 = CreateProperty(
            id: 2,
            price: 640_000m,
            area: 80m,
            neighborhood: "Tatuapé");

        var property3 = CreateProperty(
            id: 3,
            price: 900_000m,
            area: 90m,
            neighborhood: "Tatuapé");

        var property4 = CreateProperty(
            id: 4,
            price: 5_000_000m,
            area: 100m,
            neighborhood: "Mooca");

        var propertyRepository =
            new FakePropertyRepository(
            [
                property1,
                property2,
                property3,
                property4
            ]);

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var result =
            await service.GetRankingAsync(
                new OpportunityRankingRequest());

        Assert.Equal(
            3,
            result.TotalItems);

        Assert.Equal(
            1,
            result.Page);

        Assert.Equal(
            20,
            result.PageSize);

        Assert.Equal(
            1,
            result.TotalPages);

        Assert.False(
            result.HasPreviousPage);

        Assert.False(
            result.HasNextPage);

        Assert.Equal(
            3,
            result.Items.Count);

        Assert.Equal(
            1,
            result.Items[0].PropertyId);

        Assert.Equal(
            100,
            result.Items[0].OpportunityScore);

        Assert.Equal(
            "Excellent",
            result.Items[0].OpportunityLevel);

        Assert.Equal(
            -27.78m,
            result.Items[0].MarketDifferencePercentage);

        Assert.Equal(
            2,
            result.Items[0].ComparablePropertyCount);

        Assert.Equal(
            "Moderate",
            result.Items[0].ConfidenceLevel);

        Assert.DoesNotContain(
            result.Items,
            item => item.PropertyId == 4);

        Assert.True(
            result.Items[0].OpportunityScore >=
            result.Items[1].OpportunityScore);

        Assert.True(
            result.Items[1].OpportunityScore >=
            result.Items[2].OpportunityScore);
    }

    [Fact]
    public async Task GetRankingAsync_ShouldApplyPriceAndScoreFilters()
    {
        var property1 = CreateProperty(
            id: 1,
            price: 650_000m,
            area: 100m,
            neighborhood: "Tatuapé");

        var property2 = CreateProperty(
            id: 2,
            price: 640_000m,
            area: 80m,
            neighborhood: "Tatuapé");

        var property3 = CreateProperty(
            id: 3,
            price: 900_000m,
            area: 90m,
            neighborhood: "Tatuapé");

        var propertyRepository =
            new FakePropertyRepository(
            [
                property1,
                property2,
                property3
            ]);

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var request =
            new OpportunityRankingRequest
            {
                Neighborhood = "Tatuapé",
                MaxPrice = 700_000m,
                MinScore = 70
            };

        var result =
            await service.GetRankingAsync(
                request);

        var opportunity =
            Assert.Single(
                result.Items);

        Assert.Equal(
            1,
            result.TotalItems);

        Assert.Equal(
            1,
            opportunity.PropertyId);

        Assert.Equal(
            100,
            opportunity.OpportunityScore);

        Assert.Equal(
            "Excellent",
            opportunity.OpportunityLevel);

        Assert.Equal(
            2,
            opportunity.ComparablePropertyCount);

        Assert.Equal(
            "Moderate",
            opportunity.ConfidenceLevel);
    }

    [Fact]
    public async Task GetRankingAsync_ShouldUsePropertyHistory()
    {
        var property1 = CreateProperty(
            id: 1,
            price: 650_000m,
            area: 100m,
            neighborhood: "Tatuapé");

        var property2 = CreateProperty(
            id: 2,
            price: 900_000m,
            area: 100m,
            neighborhood: "Tatuapé");

        var propertyRepository =
            new FakePropertyRepository(
            [
                property1,
                property2
            ]);

        var historyRepository =
            new FakePriceHistoryRepository();

        historyRepository.HistoryByPropertyId[1] =
        [
            new PropertyPriceHistory(
                propertyId: 1,
                price: 700_000m,
                collectedAt: new DateTime(2026, 1, 1),
                sourceId: 1),

            new PropertyPriceHistory(
                propertyId: 1,
                price: 650_000m,
                collectedAt: new DateTime(2026, 2, 1),
                sourceId: 1)
        ];

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var result =
            await service.GetRankingAsync(
                new OpportunityRankingRequest());

        var propertyResult =
            Assert.Single(
                result.Items,
                item => item.PropertyId == 1);

        Assert.True(
            propertyResult.HasPriceReduction);

        Assert.NotNull(
            propertyResult.HistoricalVariationPercentage);

        Assert.Equal(
            -7.14m,
            propertyResult.HistoricalVariationPercentage);

        Assert.Equal(
            1,
            propertyResult.ComparablePropertyCount);

        Assert.Equal(
            "Low",
            propertyResult.ConfidenceLevel);
    }

    [Fact]
    public async Task GetRankingAsync_ShouldReturnFirstPage()
    {
        var propertyRepository =
            CreateRepositoryWithThreeComparableProperties();

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var request =
            new OpportunityRankingRequest
            {
                Page = 1,
                PageSize = 2
            };

        var result =
            await service.GetRankingAsync(
                request);

        Assert.Equal(
            3,
            result.TotalItems);

        Assert.Equal(
            2,
            result.TotalPages);

        Assert.Equal(
            1,
            result.Page);

        Assert.Equal(
            2,
            result.PageSize);

        Assert.Equal(
            2,
            result.Items.Count);

        Assert.Equal(
            1,
            result.Items[0].PropertyId);

        Assert.Equal(
            2,
            result.Items[1].PropertyId);

        Assert.False(
            result.HasPreviousPage);

        Assert.True(
            result.HasNextPage);
    }

    [Fact]
    public async Task GetRankingAsync_ShouldReturnSecondPage()
    {
        var propertyRepository =
            CreateRepositoryWithThreeComparableProperties();

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var request =
            new OpportunityRankingRequest
            {
                Page = 2,
                PageSize = 2
            };

        var result =
            await service.GetRankingAsync(
                request);

        Assert.Equal(
            3,
            result.TotalItems);

        Assert.Equal(
            2,
            result.TotalPages);

        Assert.Equal(
            2,
            result.Page);

        Assert.Equal(
            2,
            result.PageSize);

        var opportunity =
            Assert.Single(
                result.Items);

        Assert.Equal(
            3,
            opportunity.PropertyId);

        Assert.True(
            result.HasPreviousPage);

        Assert.False(
            result.HasNextPage);
    }

    [Fact]
    public async Task GetRankingAsync_ShouldThrow_WhenPageIsZero()
    {
        var propertyRepository =
            CreateRepositoryWithThreeComparableProperties();

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var request =
            new OpportunityRankingRequest
            {
                Page = 0
            };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetRankingAsync(request));
    }

    [Fact]
    public async Task GetRankingAsync_ShouldThrow_WhenPageSizeIsZero()
    {
        var propertyRepository =
            CreateRepositoryWithThreeComparableProperties();

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var request =
            new OpportunityRankingRequest
            {
                PageSize = 0
            };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetRankingAsync(request));
    }

    [Fact]
    public async Task GetRankingAsync_ShouldThrow_WhenPageSizeIsGreaterThanMaximum()
    {
        var propertyRepository =
            CreateRepositoryWithThreeComparableProperties();

        var historyRepository =
            new FakePriceHistoryRepository();

        var service =
            CreateService(
                propertyRepository,
                historyRepository);

        var request =
            new OpportunityRankingRequest
            {
                PageSize = 101
            };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetRankingAsync(request));
    }

    private static FakePropertyRepository
        CreateRepositoryWithThreeComparableProperties()
    {
        var property1 = CreateProperty(
            id: 1,
            price: 650_000m,
            area: 100m,
            neighborhood: "Tatuapé");

        var property2 = CreateProperty(
            id: 2,
            price: 640_000m,
            area: 80m,
            neighborhood: "Tatuapé");

        var property3 = CreateProperty(
            id: 3,
            price: 900_000m,
            area: 90m,
            neighborhood: "Tatuapé");

        return new FakePropertyRepository(
        [
            property1,
            property2,
            property3
        ]);
    }

    private static OpportunityRankingService CreateService(
        IPropertyRepository propertyRepository,
        IPropertyPriceHistoryRepository historyRepository)
    {
        var marketStatisticsService =
            new MarketStatisticsService();

        var comparisonService =
            new PropertyComparisonService(
                marketStatisticsService);

        var opportunityAnalyzer =
            new PropertyOpportunityAnalyzer(
                marketStatisticsService,
                new PriceHistoryAnalyzer(),
                new OpportunityScoreService());

        return new OpportunityRankingService(
            propertyRepository,
            historyRepository,
            comparisonService,
            opportunityAnalyzer,
            new AnalysisConfidenceService());
    }

    private static Property CreateProperty(
        int id,
        decimal price,
        decimal area,
        string neighborhood)
    {
        var property =
            new Property(
                title: $"Apartamento #{id}",
                propertyType: PropertyType.Apartment,
                transactionType: TransactionType.Sale,
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

    private class FakePropertyRepository
        : IPropertyRepository
    {
        private readonly IReadOnlyList<Property>
            _properties;

        public FakePropertyRepository(
            IReadOnlyList<Property> properties)
        {
            _properties = properties;
        }

        public Task<IReadOnlyList<Property>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _properties);
        }

        public Task<IReadOnlyList<Property>> SearchAsync(
            PropertySearchRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _properties);
        }

        public Task<Property?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var property =
                _properties.FirstOrDefault(
                    item => item.Id == id);

            return Task.FromResult(
                property);
        }

        public Task<IReadOnlyList<Property>>
            GetComparablePropertiesAsync(
                Property property,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Property> candidates =
                _properties
                    .Where(item =>
                        item.Id != property.Id)
                    .ToList();

            return Task.FromResult(
                candidates);
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
        public Dictionary<
            int,
            IReadOnlyList<PropertyPriceHistory>>
            HistoryByPropertyId { get; } = new();

        public Task<IReadOnlyList<PropertyPriceHistory>>
            GetByPropertyIdAsync(
                int propertyId,
                CancellationToken cancellationToken = default)
        {
            if (HistoryByPropertyId.TryGetValue(
                propertyId,
                out var history))
            {
                return Task.FromResult(
                    history);
            }

            IReadOnlyList<PropertyPriceHistory> empty =
                Array.Empty<PropertyPriceHistory>();

            return Task.FromResult(
                empty);
        }

        public Task AddAsync(
            PropertyPriceHistory priceHistory,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}