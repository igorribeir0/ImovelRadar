using ImovelRadar.Domain.Enums;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Domain.Tests;

public class OpportunityScoreServiceTests
{
    [Theory]
    [InlineData(-25, 100)]
    [InlineData(-20, 100)]
    [InlineData(-15, 90)]
    [InlineData(-10, 80)]
    [InlineData(-5, 70)]
    [InlineData(-1, 60)]
    [InlineData(0, 40)]
    [InlineData(5, 40)]
    [InlineData(10, 20)]
    public void CalculateFromMarketDifference_ShouldReturnExpectedScore(
        decimal percentageDifference,
        int expectedScore)
    {
        var service =
            new OpportunityScoreService();

        var result =
            service.CalculateFromMarketDifference(
                percentageDifference);

        Assert.Equal(expectedScore, result);
    }

    [Theory]
    [InlineData(0, OpportunityLevel.Low)]
    [InlineData(20, OpportunityLevel.Low)]
    [InlineData(39, OpportunityLevel.Low)]
    [InlineData(40, OpportunityLevel.Moderate)]
    [InlineData(60, OpportunityLevel.Moderate)]
    [InlineData(69, OpportunityLevel.Moderate)]
    [InlineData(70, OpportunityLevel.Good)]
    [InlineData(80, OpportunityLevel.Good)]
    [InlineData(89, OpportunityLevel.Good)]
    [InlineData(90, OpportunityLevel.Excellent)]
    [InlineData(100, OpportunityLevel.Excellent)]
    public void GetLevel_ShouldReturnExpectedLevel(
        int score,
        OpportunityLevel expectedLevel)
    {
        var service =
            new OpportunityScoreService();

        var result =
            service.GetLevel(score);

        Assert.Equal(expectedLevel, result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void GetLevel_ShouldThrow_WhenScoreIsOutsideValidRange(
        int score)
    {
        var service =
            new OpportunityScoreService();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => service.GetLevel(score));
    }
}