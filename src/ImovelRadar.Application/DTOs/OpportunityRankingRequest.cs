namespace ImovelRadar.Application.DTOs;

public class OpportunityRankingRequest
{
    public string? City { get; init; }

    public string? State { get; init; }

    public string? Neighborhood { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public int? MinScore { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}