using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Domain.Tests;

public class MarketStatisticsServiceTests
{
    [Fact]
    public void CalculateAveragePricePerSquareMeter_ShouldCalculateCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            CreateProperty(price: 600_000m, area: 75m),
            CreateProperty(price: 720_000m, area: 80m),
            CreateProperty(price: 700_000m, area: 100m)
        };

        var service = new MarketStatisticsService();

        // Act
        var average = service.CalculateAveragePricePerSquareMeter(properties);

        // Assert
        Assert.Equal(8_000m, average);
    }

    [Fact]
public void CalculateAveragePricePerSquareMeter_ShouldReturnNull_WhenListIsEmpty()
{
    // Arrange
    var properties = new List<Property>();
    var service = new MarketStatisticsService();

    // Act
    var average = service.CalculateAveragePricePerSquareMeter(properties);

    // Assert
    Assert.Null(average);
}

[Fact]
public void CalculateAveragePricePerSquareMeter_ShouldIgnorePropertyWithoutArea()
{
    // Arrange
    var properties = new List<Property>
    {
        CreateProperty(price: 600_000m, area: 75m),
        CreateProperty(price: 720_000m, area: null)
    };

    var service = new MarketStatisticsService();

    // Act
    var average = service.CalculateAveragePricePerSquareMeter(properties);

    // Assert
    Assert.Equal(8_000m, average);
}

[Fact]
public void CalculateMedianPricePerSquareMeter_ShouldCalculateCorrectly_WhenCountIsOdd()
{
    // Arrange
    var properties = new List<Property>
    {
        CreateProperty(price: 600_000m, area: 75m),  // 8.000/m²
        CreateProperty(price: 720_000m, area: 80m),  // 9.000/m²
        CreateProperty(price: 700_000m, area: 100m)  // 7.000/m²
    };

    var service = new MarketStatisticsService();

    // Act
    var median = service.CalculateMedianPricePerSquareMeter(properties);

    // Assert
    Assert.Equal(8_000m, median);
}

[Fact]
public void CalculateMedianPricePerSquareMeter_ShouldCalculateCorrectly_WhenCountIsEven()
{
    // Arrange
    var properties = new List<Property>
    {
        CreateProperty(price: 700_000m, area: 100m), // 7.000/m²
        CreateProperty(price: 600_000m, area: 75m),  // 8.000/m²
        CreateProperty(price: 720_000m, area: 80m),  // 9.000/m²
        CreateProperty(price: 1_000_000m, area: 100m) // 10.000/m²
    };

    var service = new MarketStatisticsService();

    // Act
    var median = service.CalculateMedianPricePerSquareMeter(properties);

    // Assert
    Assert.Equal(8_500m, median);
}

[Fact]
public void CalculateMedianPricePerSquareMeter_ShouldReturnNull_WhenListIsEmpty()
{
    // Arrange
    var properties = new List<Property>();
    var service = new MarketStatisticsService();

    // Act
    var median = service.CalculateMedianPricePerSquareMeter(properties);

    // Assert
    Assert.Null(median);
}

[Fact]
public void CalculateMedianPricePerSquareMeter_ShouldIgnorePropertyWithoutArea()
{
    // Arrange
    var properties = new List<Property>
    {
        CreateProperty(price: 600_000m, area: 75m), // 8.000/m²
        CreateProperty(price: 720_000m, area: null)
    };

    var service = new MarketStatisticsService();

    // Act
    var median = service.CalculateMedianPricePerSquareMeter(properties);

    // Assert
    Assert.Equal(8_000m, median);
}

[Fact]
public void CalculatePercentageDifferenceFromMarket_ShouldReturnNegativePercentage_WhenPropertyIsBelowMarket()
{
    // Arrange
    var property = CreateProperty(
        price: 700_000m,
        area: 100m);

    var service = new MarketStatisticsService();

    // Act
    var percentage = service.CalculatePercentageDifferenceFromMarket(
        property,
        marketPricePerSquareMeter: 8_000m);

    // Assert
    Assert.Equal(-12.5m, percentage);
}

private static Property CreateProperty(
    decimal price,
    decimal? area)
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
        sourceId: 1);
    }


    [Fact]
public void CalculatePercentageDifferenceFromMarket_ShouldReturnPositivePercentage_WhenPropertyIsAboveMarket()
{
    var property = CreateProperty(
        price: 900_000m,
        area: 100m);

    var service = new MarketStatisticsService();

    var percentage = service.CalculatePercentageDifferenceFromMarket(
        property,
        marketPricePerSquareMeter: 8_000m);

    Assert.Equal(12.5m, percentage);
}

[Fact]
public void CalculatePercentageDifferenceFromMarket_ShouldReturnZero_WhenPropertyMatchesMarket()
{
    var property = CreateProperty(
        price: 800_000m,
        area: 100m);

    var service = new MarketStatisticsService();

    var percentage = service.CalculatePercentageDifferenceFromMarket(
        property,
        marketPricePerSquareMeter: 8_000m);

    Assert.Equal(0m, percentage);
}

[Fact]
public void CalculatePercentageDifferenceFromMarket_ShouldReturnNull_WhenPropertyHasNoArea()
{
    var property = CreateProperty(
        price: 800_000m,
        area: null);

    var service = new MarketStatisticsService();

    var percentage = service.CalculatePercentageDifferenceFromMarket(
        property,
        marketPricePerSquareMeter: 8_000m);

    Assert.Null(percentage);
}

[Fact]
public void CalculatePercentageDifferenceFromMarket_ShouldThrowException_WhenMarketPriceIsZero()
{
    var property = CreateProperty(
        price: 800_000m,
        area: 100m);

    var service = new MarketStatisticsService();

    var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        service.CalculatePercentageDifferenceFromMarket(
            property,
            marketPricePerSquareMeter: 0m));

    Assert.Equal(
        "marketPricePerSquareMeter",
        exception.ParamName);
}

}