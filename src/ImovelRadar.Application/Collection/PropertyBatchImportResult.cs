using ImovelRadar.Application.Collection.Enums;

namespace ImovelRadar.Application.Collection;

public class PropertyBatchImportResult
{
    public PropertyBatchImportResult(
        IReadOnlyList<PropertyImportResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        Results = results;

        Total = results.Count;

        Created = results.Count(
            result =>
                result.Status ==
                PropertyImportStatus.Created);

        Updated = results.Count(
            result =>
                result.Status ==
                PropertyImportStatus.Updated);

        Unchanged = results.Count(
            result =>
                result.Status ==
                PropertyImportStatus.Unchanged);

        Rejected = results.Count(
            result =>
                result.Status ==
                PropertyImportStatus.Rejected);
    }

    public int Total { get; }

    public int Created { get; }

    public int Updated { get; }

    public int Unchanged { get; }

    public int Rejected { get; }

    public IReadOnlyList<PropertyImportResult> Results { get; }
}