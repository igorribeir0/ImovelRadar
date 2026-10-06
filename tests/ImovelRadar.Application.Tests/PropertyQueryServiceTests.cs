using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Application.Services;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Application.Tests;

public class PropertyQueryServiceTests
{
    [Fact]
    public async Task SearchAsync_ShouldReturnProperties()
    {
        var repository = new FakePropertyRepository
        {
            SearchResults =
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
                    neighborhood: "Tatuapé")
            ]
        };

        var service =
            new PropertyQueryService(repository);

        var request = new PropertySearchRequest
        {
            Neighborhood = "Tatuapé",
            MaxPrice = 700_000m
        };

        var result =
            await service.SearchAsync(request);

        Assert.Equal(
            2,
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

        Assert.Equal(
            2,
            result.Items.Count);

        Assert.Equal(
            1,
            result.Items[0].Id);

        Assert.Equal(
            2,
            result.Items[1].Id);

        Assert.All(
            result.Items,
            property =>
                Assert.Equal(
                    "Tatuapé",
                    property.Neighborhood));

        Assert.False(
            result.HasPreviousPage);

        Assert.False(
            result.HasNextPage);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnEmptyList_WhenNoPropertyMatches()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyQueryService(repository);

        var request = new PropertySearchRequest
        {
            Neighborhood = "Pinheiros"
        };

        var result =
            await service.SearchAsync(request);

        Assert.Empty(
            result.Items);

        Assert.Equal(
            0,
            result.TotalItems);

        Assert.Equal(
            0,
            result.TotalPages);

        Assert.False(
            result.HasPreviousPage);

        Assert.False(
            result.HasNextPage);
    }

    [Fact]
    public async Task SearchAsync_ShouldThrow_WhenMinPriceIsGreaterThanMaxPrice()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyQueryService(repository);

        var request = new PropertySearchRequest
        {
            MinPrice = 900_000m,
            MaxPrice = 600_000m
        };

        var exception =
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.SearchAsync(request));

        Assert.Equal(
            "O preço mínimo não pode ser maior que o preço máximo.",
            exception.Message);
    }

    [Fact]
    public async Task SearchAsync_ShouldThrow_WhenMinAreaIsGreaterThanMaxArea()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyQueryService(repository);

        var request = new PropertySearchRequest
        {
            MinArea = 120m,
            MaxArea = 80m
        };

        var exception =
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.SearchAsync(request));

        Assert.Equal(
            "A área mínima não pode ser maior que a área máxima.",
            exception.Message);
    }

    [Fact]
    public async Task SearchAsync_ShouldThrow_WhenMinPriceIsNegative()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyQueryService(repository);

        var request = new PropertySearchRequest
        {
            MinPrice = -1m
        };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.SearchAsync(request));
    }

    [Fact]
    public async Task SearchAsync_ShouldThrow_WhenBedroomsIsNegative()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyQueryService(repository);

        var request = new PropertySearchRequest
        {
            Bedrooms = -1
        };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.SearchAsync(request));
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnFirstPage()
    {
        var repository =
            CreateRepositoryWithFourProperties();

        var service =
            new PropertyQueryService(repository);

        var request =
            new PropertySearchRequest
            {
                Page = 1,
                PageSize = 2
            };

        var result =
            await service.SearchAsync(request);

        Assert.Equal(
            4,
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
            result.Items[0].Id);

        Assert.Equal(
            2,
            result.Items[1].Id);

        Assert.False(
            result.HasPreviousPage);

        Assert.True(
            result.HasNextPage);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnSecondPage()
    {
        var repository =
            CreateRepositoryWithFourProperties();

        var service =
            new PropertyQueryService(repository);

        var request =
            new PropertySearchRequest
            {
                Page = 2,
                PageSize = 2
            };

        var result =
            await service.SearchAsync(request);

        Assert.Equal(
            4,
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

        Assert.Equal(
            2,
            result.Items.Count);

        Assert.Equal(
            3,
            result.Items[0].Id);

        Assert.Equal(
            4,
            result.Items[1].Id);

        Assert.True(
            result.HasPreviousPage);

        Assert.False(
            result.HasNextPage);
    }

    [Fact]
    public async Task SearchAsync_ShouldThrow_WhenPageIsZero()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyQueryService(repository);

        var request =
            new PropertySearchRequest
            {
                Page = 0
            };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.SearchAsync(request));
    }

    [Fact]
    public async Task SearchAsync_ShouldThrow_WhenPageSizeIsZero()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyQueryService(repository);

        var request =
            new PropertySearchRequest
            {
                PageSize = 0
            };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.SearchAsync(request));
    }

    [Fact]
    public async Task SearchAsync_ShouldThrow_WhenPageSizeIsGreaterThanMaximum()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyQueryService(repository);

        var request =
            new PropertySearchRequest
            {
                PageSize = 101
            };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.SearchAsync(request));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProperty()
    {
        var repository = new FakePropertyRepository
        {
            Property = CreateProperty(
                id: 1,
                price: 650_000m,
                area: 100m,
                neighborhood: "Tatuapé")
        };

        var service =
            new PropertyQueryService(repository);

        var result =
            await service.GetByIdAsync(1);

        Assert.Equal(
            1,
            result.Id);

        Assert.Equal(
            650_000m,
            result.Price);

        Assert.Equal(
            100m,
            result.Area);

        Assert.Equal(
            6_500m,
            result.PricePerSquareMeter);

        Assert.Equal(
            "Tatuapé",
            result.Neighborhood);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenPropertyDoesNotExist()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyQueryService(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GetByIdAsync(999));
    }

    private static FakePropertyRepository
        CreateRepositoryWithFourProperties()
    {
        return new FakePropertyRepository
        {
            SearchResults =
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
            ]
        };
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
        public Property? Property { get; set; }

        public IReadOnlyList<Property> SearchResults { get; set; }
            = Array.Empty<Property>();

        public Task<IReadOnlyList<Property>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                SearchResults);
        }

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
            return Task.FromResult(
                SearchResults);
        }

        public Task<Property?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                Property);
        }

        public Task<IReadOnlyList<Property>>
            GetComparablePropertiesAsync(
                Property property,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Property> properties =
                Array.Empty<Property>();

            return Task.FromResult(
                properties);
        }

        public Task UpdateAsync(
            Property property,
            CancellationToken cancellationToken = default)
        {
            Property = property;

            return Task.CompletedTask;
        }
    }
}