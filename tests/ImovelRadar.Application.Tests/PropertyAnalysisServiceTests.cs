using ImovelRadar.Application.Interfaces;
using ImovelRadar.Application.Services;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;
using ImovelRadar.Domain.Services;
using ImovelRadar.Application.DTOs;

namespace ImovelRadar.Application.Tests;

public class PropertyAnalysisServiceTests
{
    [Fact]
    public async Task AnalyzePropertyAsync_ShouldReturnOpportunityAnalysis()
    {
        var property = CreateProperty(
            price: 650_000m,
            area: 100m,
            neighborhood: "Tatuapé");

        var propertyRepository = new FakePropertyRepository
        {
            Property = property,
            ComparableProperties =
            [
                // 8.000/m²
                CreateProperty(
                    640_000m,
                    80m,
                    "Tatuapé"),

                // 10.000/m²
                CreateProperty(
                    900_000m,
                    90m,
                    "Tatuapé"),

                // Não deve participar: outro bairro
                CreateProperty(
                    5_000_000m,
                    100m,
                    "Mooca")
            ]
        };

        var historyRepository =
            new FakePropertyPriceHistoryRepository();

        historyRepository.Items.Add(
            CreateHistory(
                700_000m,
                new DateTime(2026, 1, 1)));

        historyRepository.Items.Add(
            CreateHistory(
                650_000m,
                new DateTime(2026, 2, 1)));

        var service = CreateService(
            propertyRepository,
            historyRepository);

        var result =
            await service.AnalyzePropertyAsync(1);

        Assert.Equal(
            650_000m,
            result.PropertyPrice);

        Assert.Equal(
            6_500m,
            result.PricePerSquareMeter);

        Assert.Equal(
            9_000m,
            result.MarketPricePerSquareMeter);

        Assert.Equal(
            700_000m,
            result.InitialPrice);

        Assert.True(
            result.HasPriceReduction);

        Assert.Equal(
            1,
            result.PriceChangeCount);

        Assert.Equal(
            100,
            result.OpportunityScore);
    }

    [Fact]
    public async Task AnalyzePropertyAsync_ShouldThrow_WhenPropertyDoesNotExist()
    {
        var propertyRepository =
            new FakePropertyRepository();

        var historyRepository =
            new FakePropertyPriceHistoryRepository();

        var service = CreateService(
            propertyRepository,
            historyRepository);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AnalyzePropertyAsync(999));
    }

    [Fact]
    public async Task AnalyzePropertyAsync_ShouldThrow_WhenThereAreNoComparables()
    {
        var propertyRepository =
            new FakePropertyRepository
            {
                Property = CreateProperty(
                    650_000m,
                    100m,
                    "Tatuapé")
            };

        var historyRepository =
            new FakePropertyPriceHistoryRepository();

        var service = CreateService(
            propertyRepository,
            historyRepository);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.AnalyzePropertyAsync(1));

        Assert.Equal(
            "Não existem imóveis comparáveis suficientes para calcular a referência de mercado.",
            exception.Message);
    }

    [Fact]
    public async Task AnalyzePropertyAsync_ShouldThrow_WhenPropertyIdIsInvalid()
    {
        var service = CreateService(
            new FakePropertyRepository(),
            new FakePropertyPriceHistoryRepository());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.AnalyzePropertyAsync(0));
    }

    private static PropertyAnalysisService CreateService(
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

        return new PropertyAnalysisService(
            propertyRepository,
            historyRepository,
            comparisonService,
            opportunityAnalyzer);
    }

    private static Property CreateProperty(
        decimal price,
        decimal? area,
        string? neighborhood)
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
            sourceId: 1,
            neighborhood: neighborhood);
    }

    private static PropertyPriceHistory CreateHistory(
        decimal price,
        DateTime date)
    {
        return new PropertyPriceHistory(
            propertyId: 1,
            price: price,
            collectedAt: date,
            sourceId: 1);
    }

    private class FakePropertyRepository
    : IPropertyRepository
{
    public Property? Property { get; set; }

    public IReadOnlyList<Property> ComparableProperties { get; set; }
        = Array.Empty<Property>();

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
        return Task.FromResult(
            ComparableProperties);
    }

    public Task UpdateAsync(
        Property property,
        CancellationToken cancellationToken = default)
    {
        Property = property;

        return Task.CompletedTask;
    }
}

    private class FakePropertyPriceHistoryRepository
        : IPropertyPriceHistoryRepository
    {
        public List<PropertyPriceHistory> Items { get; } = [];

        public Task<IReadOnlyList<PropertyPriceHistory>>
            GetByPropertyIdAsync(
                int propertyId,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<PropertyPriceHistory> result = Items;

            return Task.FromResult(result);
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