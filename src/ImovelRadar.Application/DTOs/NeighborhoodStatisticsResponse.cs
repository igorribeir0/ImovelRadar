namespace ImovelRadar.Application.DTOs;

public class NeighborhoodStatisticsResponse
{
    public string City { get; init; } = null!;
    public string State { get; init; } = null!;
    public string Neighborhood { get; init; } = null!;

    public int PropertyCount { get; init; }

    public decimal AveragePrice { get; init; }
    public decimal MedianPrice { get; init; }

    public decimal? AveragePricePerSquareMeter { get; init; }
    public decimal? MedianPricePerSquareMeter { get; init; }

    public decimal LowestPrice { get; init; }
    public decimal HighestPrice { get; init; }
}