using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Application.Services;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Application.Tests;

public class NeighborhoodStatisticsServiceTests
{
    [Fact]
    public async Task GetAsync_ShouldCalculateNeighborhoodStatistics()
    {
        var repository = new FakePropertyRepository
        {
            SearchResults =
            [
                CreateProperty(
                    650_000m,
                    100m,
                    "Tatuapé"),

                CreateProperty(
                    640_000m,
                    80m,
                    "Tatuapé"),

                CreateProperty(
                    900_000m,
                    90m,
                    "Tatuapé")
            ]
        };

        var service = new NeighborhoodStatisticsService(
            repository,
            new MarketStatisticsService());

        var result = await service.GetAsync(
            city: "São Paulo",
            state: "SP",
            neighborhood: "Tatuapé");

        Assert.Equal("São Paulo", result.City);
        Assert.Equal("SP", result.State);
        Assert.Equal("Tatuapé", result.Neighborhood);

        Assert.Equal(3, result.PropertyCount);

        Assert.Equal(
            730_000m,
            result.AveragePrice);

        Assert.Equal(
            650_000m,
            result.MedianPrice);

        Assert.Equal(
            8_166.666666666666666666666667m,
            result.AveragePricePerSquareMeter);

        Assert.Equal(
            8_000m,
            result.MedianPricePerSquareMeter);

        Assert.Equal(
            640_000m,
            result.LowestPrice);

        Assert.Equal(
            900_000m,
            result.HighestPrice);
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenNoPropertiesExist()
    {
        var repository =
            new FakePropertyRepository();

        var service = new NeighborhoodStatisticsService(
            repository,
            new MarketStatisticsService());

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetAsync(
                    "São Paulo",
                    "SP",
                    "Pinheiros"));

        Assert.Equal(
            "Não existem imóveis para calcular as estatísticas do bairro.",
            exception.Message);
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenCityIsEmpty()
    {
        var service = new NeighborhoodStatisticsService(
            new FakePropertyRepository(),
            new MarketStatisticsService());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetAsync(
                "",
                "SP",
                "Tatuapé"));
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenStateIsEmpty()
    {
        var service = new NeighborhoodStatisticsService(
            new FakePropertyRepository(),
            new MarketStatisticsService());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetAsync(
                "São Paulo",
                "",
                "Tatuapé"));
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenNeighborhoodIsEmpty()
    {
        var service = new NeighborhoodStatisticsService(
            new FakePropertyRepository(),
            new MarketStatisticsService());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetAsync(
                "São Paulo",
                "SP",
                ""));
    }

    private static Property CreateProperty(
        decimal price,
        decimal area,
        string neighborhood)
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

    private class FakePropertyRepository
        : IPropertyRepository
    {
        public IReadOnlyList<Property> SearchResults { get; set; }
            = Array.Empty<Property>();

        public Task<IReadOnlyList<Property>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(SearchResults);
        }

        public Task<IReadOnlyList<Property>> SearchAsync(
            PropertySearchRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(SearchResults);
        }

        public Task<Property?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Property?>(null);
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
            return Task.CompletedTask;
        }
    }
}