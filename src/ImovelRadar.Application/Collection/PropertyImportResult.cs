using ImovelRadar.Application.Collection.Enums;
using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Application.Collection;

public class PropertyImportResult
{
    private PropertyImportResult(
        PropertyImportStatus status,
        Property? property,
        IReadOnlyList<string> errors)
    {
        Status = status;
        Property = property;
        Errors = errors;
    }

    public PropertyImportStatus Status { get; }

    public Property? Property { get; }

    public IReadOnlyList<string> Errors { get; }

    public bool IsSuccess =>
        Status != PropertyImportStatus.Rejected;

    public static PropertyImportResult Created(
        Property property)
    {
        ArgumentNullException.ThrowIfNull(property);

        return new PropertyImportResult(
            PropertyImportStatus.Created,
            property,
            Array.Empty<string>());
    }

    public static PropertyImportResult Updated(
        Property property)
    {
        ArgumentNullException.ThrowIfNull(property);

        return new PropertyImportResult(
            PropertyImportStatus.Updated,
            property,
            Array.Empty<string>());
    }

    public static PropertyImportResult Unchanged(
        Property property)
    {
        ArgumentNullException.ThrowIfNull(property);

        return new PropertyImportResult(
            PropertyImportStatus.Unchanged,
            property,
            Array.Empty<string>());
    }

    public static PropertyImportResult Rejected(
        IEnumerable<string> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var errorList =
            errors
                .Where(error =>
                    !string.IsNullOrWhiteSpace(error))
                .ToList();

        if (errorList.Count == 0)
        {
            throw new ArgumentException(
                "Uma importação rejeitada deve possuir pelo menos um erro.",
                nameof(errors));
        }

        return new PropertyImportResult(
            PropertyImportStatus.Rejected,
            null,
            errorList);
    }
}