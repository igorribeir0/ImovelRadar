using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Tests;

public class PropertyTests
{
[Fact]
    public void GetPricePerSquareMeter_ShouldCalculateCorrectly()
{
    // Arrange
    var property = CreateValidProperty();

    // Act
    var pricePerSquareMeter = property.GetPricePerSquareMeter();

    // Assert
    Assert.Equal(8_000m, pricePerSquareMeter);
}

[Fact]
    public void Constructor_ShouldThrowException_WhenPriceIsZero()
    {
    // Act
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        new Property(
            title: "Apartamento em São Paulo",
            propertyType: PropertyType.Apartment,
            transactionType: TransactionType.Sale,
            price: 0m,
            area: 75m,
            bedrooms: 2,
            bathrooms: 2,
            parkingSpaces: 1,
            city: "São Paulo",
            state: "SP",
            sourceId: 1));

    // Assert
    Assert.Equal("price", exception.ParamName);
    }


    private static Property CreateValidProperty(
    decimal price = 600_000m,
    decimal? area = 75m,
    string city = "São Paulo",
    string state = "SP",
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
        city: city,
        state: state,
        sourceId: sourceId);
    }




[Fact]
    public void Constructor_ShouldThrowException_WhenAreaIsZero()
    {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        CreateValidProperty(area: 0m));

    Assert.Equal("area", exception.ParamName);
    }

[Fact]
    public void Constructor_ShouldThrowException_WhenCityIsEmpty()
    {
    var exception = Assert.Throws<ArgumentException>(() =>
        CreateValidProperty(city: ""));

    Assert.Equal("city", exception.ParamName);
    }



[Fact]

    public void Constructor_ShouldThrowException_WhenStateIsEmpty()
    {
    var exception = Assert.Throws<ArgumentException>(() =>
        CreateValidProperty(state: ""));

    Assert.Equal("state", exception.ParamName);
    }


[Fact]
    public void Constructor_ShouldThrowException_WhenSourceIdIsZero()
    {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        CreateValidProperty(sourceId: 0));

    Assert.Equal("sourceId", exception.ParamName);
    }

[Fact]
public void UpdatePrice_ShouldUpdatePropertyPrice()
{
    // Arrange
    var property = CreateValidProperty(price: 600_000m);

    // Act
    property.UpdatePrice(570_000m);

    // Assert
    Assert.Equal(570_000m, property.Price);
}


[Fact]
public void UpdatePrice_ShouldThrowException_WhenNewPriceIsZero()
{
    // Arrange
    var property = CreateValidProperty();

    // Act
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        property.UpdatePrice(0m));

    // Assert
    Assert.Equal("newPrice", exception.ParamName);
}

}