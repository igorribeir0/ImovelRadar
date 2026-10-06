using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Services;

public class OpportunityScoreService
{
    public int CalculateFromMarketDifference(
        decimal percentageDifference)
    {
        if (percentageDifference <= -20m) return 100;
        if (percentageDifference <= -15m) return 90;
        if (percentageDifference <= -10m) return 80;
        if (percentageDifference <= -5m) return 70;
        if (percentageDifference < 0m) return 60;
        if (percentageDifference <= 5m) return 40;

        return 20;
    }

    public OpportunityLevel GetLevel(int score)
    {
        if (score < 0 || score > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(score),
                "O score deve estar entre 0 e 100.");
        }

        if (score >= 90)
            return OpportunityLevel.Excellent;

        if (score >= 70)
            return OpportunityLevel.Good;

        if (score >= 40)
            return OpportunityLevel.Moderate;

        return OpportunityLevel.Low;
    }
}