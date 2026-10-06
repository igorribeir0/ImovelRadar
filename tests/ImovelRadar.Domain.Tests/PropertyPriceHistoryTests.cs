using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Domain.Tests;

public class PropertyPriceHistoryTests
{
    [Fact]
    public void Constructor_ShouldCreatePriceHistory()
    {
        // Arrange
        var collectedAt = DateTime.UtcNow;

        // Act
        var history = new PropertyPriceHistory(
            propertyId: 1,
            price: 600_000m,
            collectedAt: collectedAt,
            sourceId: 1);

        // Assert
        Assert.Equal(1, history.PropertyId);
        Assert.Equal(600_000m, history.Price);
        Assert.Equal(collectedAt, history.CollectedAt);
        Assert.Equal(1, history.SourceId);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenPropertyIdIsZero()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PropertyPriceHistory(
                propertyId: 0,
                price: 600_000m,
                collectedAt: DateTime.UtcNow,
                sourceId: 1));

        Assert.Equal("propertyId", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenPriceIsZero()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PropertyPriceHistory(
                propertyId: 1,
                price: 0m,
                collectedAt: DateTime.UtcNow,
                sourceId: 1));

        Assert.Equal("price", exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenSourceIdIsZero()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PropertyPriceHistory(
                propertyId: 1,
                price: 600_000m,
                collectedAt: DateTime.UtcNow,
                sourceId: 0));

        Assert.Equal("sourceId", exception.ParamName);
    }
}