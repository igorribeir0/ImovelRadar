namespace ImovelRadar.Application.Collection;

public class PropertyBatchImportService
{
    private readonly PropertyImportService
        _propertyImportService;

    public PropertyBatchImportService(
        PropertyImportService propertyImportService)
    {
        _propertyImportService =
            propertyImportService;
    }

    public async Task<PropertyBatchImportResult> ImportAsync(
        IEnumerable<RawProperty> rawProperties,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rawProperties);

        var results =
            new List<PropertyImportResult>();

        foreach (var rawProperty in rawProperties)
        {
            cancellationToken
                .ThrowIfCancellationRequested();

            var result =
                await _propertyImportService.ImportAsync(
                    rawProperty,
                    cancellationToken);

            results.Add(result);
        }

        return new PropertyBatchImportResult(
            results);
    }
}