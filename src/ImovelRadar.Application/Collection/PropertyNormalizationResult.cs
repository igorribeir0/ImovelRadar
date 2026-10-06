using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Application.Collection;

public class PropertyNormalizationResult
{
    private PropertyNormalizationResult(
        Property? property,
        IReadOnlyList<string> errors)
    {
        Property = property;
        Errors = errors;
    }

    public bool IsSuccess =>
        Property is not null &&
        Errors.Count == 0;

    public Property? Property { get; }

    public IReadOnlyList<string> Errors { get; }

    public static PropertyNormalizationResult Success(
        Property property)
    {
        ArgumentNullException.ThrowIfNull(property);

        return new PropertyNormalizationResult(
            property,
            Array.Empty<string>());
    }

    public static PropertyNormalizationResult Failure(
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
                "Uma falha de normalização deve possuir pelo menos um erro.",
                nameof(errors));
        }

        return new PropertyNormalizationResult(
            null,
            errorList);
    }
}