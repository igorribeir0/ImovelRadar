namespace ImovelRadar.Domain.Entities;

public class Source
{
    public int Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string? BaseUrl { get; private set; }

    public string Type { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
}