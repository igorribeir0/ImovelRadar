using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Domain.Tests;

public class PropertyComparisonServiceTests
{
    private readonly PropertyComparisonService _service =
        new(new MarketStatisticsService());

    [Fact]
    public void IsComparable_ShouldReturnTrue_WhenPropertiesMatchCriteria()
    {
        var reference = CreateProperty(
            price: 600_000m,
            area: 75m,
            neighborhood: "Tatuapé");

        var candidate = CreateProperty(
            price: 640_000m,
            area: 80m,
            neighborhood: "Tatuapé");

        Assert.True(
            _service.IsComparable(reference, candidate));
    }

    [Fact]
    public void IsComparable_ShouldReturnFalse_WhenNeighborhoodIsDifferent()
    {
        var reference = CreateProperty(
            600_000m,
            75m,
            "Tatuapé");

        var candidate = CreateProperty(
            600_000m,
            75m,
            "Mooca");

        Assert.False(
            _service.IsComparable(reference, candidate));
    }

    [Fact]
    public void IsComparable_ShouldReturnFalse_WhenBedroomsAreDifferent()
    {
        var reference = CreateProperty(
            600_000m,
            75m,
            "Tatuapé",
            bedrooms: 2);

        var candidate = CreateProperty(
            600_000m,
            75m,
            "Tatuapé",
            bedrooms: 3);

        Assert.False(
            _service.IsComparable(reference, candidate));
    }

    [Fact]
    public void IsComparable_ShouldReturnFalse_WhenNeighborhoodIsMissing()
    {
        var reference = CreateProperty(
            600_000m,
            75m,
            null);

        var candidate = CreateProperty(
            600_000m,
            75m,
            "Tatuapé");

        Assert.False(
            _service.IsComparable(reference, candidate));
    }

    [Fact]
    public void FilterComparables_ShouldReturnOnlyValidProperties()
    {
        var reference = CreateProperty(
            600_000m,
            75m,
            "Tatuapé");

        var candidates = new List<Property>
        {
            CreateProperty(
                640_000m,
                80m,
                "Tatuapé"),

            CreateProperty(
                700_000m,
                80m,
                "Mooca"),

            CreateProperty(
                800_000m,
                90m,
                "Tatuapé",
                bedrooms: 3)
        };

        var result =
            _service.FilterComparables(
                reference,
                candidates);

        Assert.Single(result);
        Assert.Equal(
            640_000m,
            result[0].Price);
    }

    [Fact]
    public void CalculateMedianPricePerSquareMeter_ShouldUseOnlyComparables()
    {
        var reference = CreateProperty(
            600_000m,
            75m,
            "Tatuapé");

        var candidates = new List<Property>
        {
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

            // 50.000/m² - deve ser ignorado
            CreateProperty(
                5_000_000m,
                100m,
                "Mooca")
        };

        var median =
            _service.CalculateMedianPricePerSquareMeter(
                reference,
                candidates);

        Assert.Equal(
            9_000m,
            median);
    }

    private static Property CreateProperty(
        decimal price,
        decimal? area,
        string? neighborhood,
        int? bedrooms = 2)
    {
        return new Property(
            title: "Apartamento em São Paulo",
            propertyType: PropertyType.Apartment,
            transactionType: TransactionType.Sale,
            price: price,
            area: area,
            bedrooms: bedrooms,
            bathrooms: 2,
            parkingSpaces: 1,
            city: "São Paulo",
            state: "SP",
            sourceId: 1,
            neighborhood: neighborhood);
    }
}