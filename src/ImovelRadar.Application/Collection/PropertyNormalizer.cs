using System.Globalization;
using System.Text.RegularExpressions;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Application.Collection;

public class PropertyNormalizer
{
    private static readonly CultureInfo BrazilianCulture =
        CultureInfo.GetCultureInfo("pt-BR");

    public PropertyNormalizationResult Normalize(
        RawProperty rawProperty)
    {
        ArgumentNullException.ThrowIfNull(rawProperty);

        var errors =
            new List<string>();

        if (rawProperty.SourceId <= 0)
        {
            errors.Add(
                "A origem do imóvel é obrigatória.");
        }

        var price =
            ParseDecimal(
                rawProperty.Price);

        if (!price.HasValue ||
            price.Value <= 0)
        {
            errors.Add(
                "O preço do imóvel é inválido.");
        }

        var area =
            ParseDecimal(
                rawProperty.Area);

        if (!string.IsNullOrWhiteSpace(
                rawProperty.Area) &&
            (!area.HasValue ||
             area.Value <= 0))
        {
            errors.Add(
                "A área do imóvel é inválida.");
        }

        if (string.IsNullOrWhiteSpace(
                rawProperty.City))
        {
            errors.Add(
                "A cidade é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(
                rawProperty.State))
        {
            errors.Add(
                "O estado é obrigatório.");
        }

        var propertyType =
            ParsePropertyType(
                rawProperty.PropertyType);

        if (!propertyType.HasValue)
        {
            errors.Add(
                "O tipo do imóvel é inválido.");
        }

        var transactionType =
            ParseTransactionType(
                rawProperty.TransactionType);

        if (!transactionType.HasValue)
        {
            errors.Add(
                "O tipo da transação é inválido.");
        }

        var bedrooms =
            ParseInteger(
                rawProperty.Bedrooms);

        var bathrooms =
            ParseInteger(
                rawProperty.Bathrooms);

        var parkingSpaces =
            ParseInteger(
                rawProperty.ParkingSpaces);

        if (!IsValidOptionalInteger(
                rawProperty.Bedrooms,
                bedrooms))
        {
            errors.Add(
                "A quantidade de quartos é inválida.");
        }

        if (!IsValidOptionalInteger(
                rawProperty.Bathrooms,
                bathrooms))
        {
            errors.Add(
                "A quantidade de banheiros é inválida.");
        }

        if (!IsValidOptionalInteger(
                rawProperty.ParkingSpaces,
                parkingSpaces))
        {
            errors.Add(
                "A quantidade de vagas é inválida.");
        }

        if (errors.Count > 0)
        {
            return PropertyNormalizationResult
                .Failure(errors);
        }

        var property =
            new Property(
                title:
                    NormalizeOptionalText(
                        rawProperty.Title),

                propertyType:
                    propertyType!.Value,

                transactionType:
                    transactionType!.Value,

                price:
                    price!.Value,

                area:
                    area,

                bedrooms:
                    bedrooms,

                bathrooms:
                    bathrooms,

                parkingSpaces:
                    parkingSpaces,

                city:
                    rawProperty.City!.Trim(),

                state:
                    rawProperty.State!
                        .Trim()
                        .ToUpperInvariant(),

                sourceId:
                    rawProperty.SourceId,

                neighborhood:
                    NormalizeOptionalText(
                        rawProperty.Neighborhood),

                externalId:
                    NormalizeOptionalText(
                        rawProperty.ExternalId),

                sourceUrl:
                    NormalizeOptionalText(
                        rawProperty.SourceUrl));

        return PropertyNormalizationResult
            .Success(property);
    }

    private static decimal? ParseDecimal(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized =
            value.Trim();

        normalized =
            Regex.Replace(
                normalized,
                @"[^\d,.\-]",
                string.Empty);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        if (decimal.TryParse(
                normalized,
                NumberStyles.Number |
                NumberStyles.AllowCurrencySymbol,
                BrazilianCulture,
                out var brazilianValue))
        {
            return brazilianValue;
        }

        if (decimal.TryParse(
                normalized,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var invariantValue))
        {
            return invariantValue;
        }

        return null;
    }

    private static int? ParseInteger(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var match =
            Regex.Match(
                value,
                @"-?\d+");

        if (!match.Success)
        {
            return null;
        }

        return int.TryParse(
            match.Value,
            out var result)
                ? result
                : null;
    }

    private static bool IsValidOptionalInteger(
        string? rawValue,
        int? parsedValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return true;
        }

        return parsedValue.HasValue &&
               parsedValue.Value >= 0;
    }

    private static PropertyType? ParsePropertyType(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized =
            value.Trim();

        if (normalized.Equals(
                "Apartment",
                StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals(
                "Apartamento",
                StringComparison.OrdinalIgnoreCase))
        {
            return PropertyType.Apartment;
        }

        return null;
    }

    private static TransactionType? ParseTransactionType(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized =
            value.Trim();

        if (normalized.Equals(
                "Sale",
                StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals(
                "Venda",
                StringComparison.OrdinalIgnoreCase))
        {
            return TransactionType.Sale;
        }

        if (normalized.Equals(
                "Rent",
                StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals(
                "Aluguel",
                StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals(
                "Locação",
                StringComparison.OrdinalIgnoreCase))
        {
            return TransactionType.Rent;
        }

        return null;
    }

    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}