using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Entities;

public class Source
{
    public Source(
        string name,
        SourceType type,
        string? baseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome da fonte é obrigatório.",
                nameof(name));
        }

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                "O tipo da fonte é inválido.");
        }

        Name = name.Trim();
        Type = type;

        BaseUrl = string.IsNullOrWhiteSpace(baseUrl)
            ? null
            : baseUrl.Trim();

        IsActive = true;

        var now = DateTime.UtcNow;

        CreatedAt = now;
        UpdatedAt = now;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }

    public string? BaseUrl { get; private set; }

    public SourceType Type { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(id),
                "O identificador da fonte deve ser maior que zero.");
        }

        if (Id != 0)
        {
            throw new InvalidOperationException(
                "A fonte já possui um identificador.");
        }

        Id = id;
    }
}