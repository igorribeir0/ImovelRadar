using ImovelRadar.Domain.Enums;
using ImovelRadar.Domain.Services;

namespace ImovelRadar.Domain.Tests;

public class AnalysisConfidenceServiceTests
{
    [Theory]
    [InlineData(0, ConfidenceLevel.Low)]
    [InlineData(1, ConfidenceLevel.Low)]
    [InlineData(2, ConfidenceLevel.Moderate)]
    [InlineData(4, ConfidenceLevel.Moderate)]
    [InlineData(5, ConfidenceLevel.Good)]
    [InlineData(9, ConfidenceLevel.Good)]
    [InlineData(10, ConfidenceLevel.High)]
    [InlineData(50, ConfidenceLevel.High)]
    public void Calculate_ShouldReturnExpectedConfidenceLevel(
        int comparablePropertyCount,
        ConfidenceLevel expectedLevel)
    {
        var service =
            new AnalysisConfidenceService();

        var result =
            service.Calculate(
                comparablePropertyCount);

        Assert.Equal(
            expectedLevel,
            result);
    }

    [Fact]
    public void Calculate_ShouldThrow_WhenCountIsNegative()
    {
        var service =
            new AnalysisConfidenceService();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => service.Calculate(-1));
    }
}