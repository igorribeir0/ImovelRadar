using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Services;

public class AnalysisConfidenceService
{
    public ConfidenceLevel Calculate(
        int comparablePropertyCount)
    {
        if (comparablePropertyCount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(comparablePropertyCount),
                "A quantidade de imóveis comparáveis não pode ser negativa.");
        }

        if (comparablePropertyCount >= 10)
            return ConfidenceLevel.High;

        if (comparablePropertyCount >= 5)
            return ConfidenceLevel.Good;

        if (comparablePropertyCount >= 2)
            return ConfidenceLevel.Moderate;

        return ConfidenceLevel.Low;
    }
}