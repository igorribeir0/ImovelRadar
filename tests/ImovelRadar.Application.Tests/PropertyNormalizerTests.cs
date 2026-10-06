using ImovelRadar.Application.Collection;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Application.Tests;

public class PropertyNormalizerTests
{
    [Fact]
    public void Normalize_ShouldNormalizeValidProperty()
    {
        var normalizer =
            new PropertyNormalizer();

        var rawProperty =
            new RawProperty
            {
                SourceId = 1,
                ExternalId = " ABC-123 ",
                SourceUrl =
                    " https://example.com/imovel/123 ",
                Title =
                    " Apartamento no Tatuapé ",
                PropertyType =
                    "Apartamento",
                TransactionType =
                    "Venda",
                Price =
                    "R$ 650.000,00",
                Area =
                    "100 m²",
                Bedrooms =
                    "2 quartos",
                Bathrooms =
                    "2 banheiros",
                ParkingSpaces =
                    "1 vaga",
                Neighborhood =
                    " Tatuapé ",
                City =
                    " São Paulo ",
                State =
                    " sp "
            };

        var result =
            normalizer.Normalize(
                rawProperty);

        Assert.True(
            result.IsSuccess);

        Assert.Empty(
            result.Errors);

        Assert.NotNull(
            result.Property);

        var property =
            result.Property!;

        Assert.Equal(
            650_000m,
            property.Price);

        Assert.Equal(
            100m,
            property.Area);

        Assert.Equal(
            2,
            property.Bedrooms);

        Assert.Equal(
            2,
            property.Bathrooms);

        Assert.Equal(
            1,
            property.ParkingSpaces);

        Assert.Equal(
            "Tatuapé",
            property.Neighborhood);

        Assert.Equal(
            "São Paulo",
            property.City);

        Assert.Equal(
            "SP",
            property.State);

        Assert.Equal(
            "ABC-123",
            property.ExternalId);

        Assert.Equal(
            "https://example.com/imovel/123",
            property.SourceUrl);

        Assert.Equal(
            PropertyType.Apartment,
            property.PropertyType);

        Assert.Equal(
            TransactionType.Sale,
            property.TransactionType);
    }

    [Fact]
    public void Normalize_ShouldAllowMissingOptionalFields()
    {
        var normalizer =
            new PropertyNormalizer();

        var rawProperty =
            new RawProperty
            {
                SourceId = 1,
                PropertyType = "Apartamento",
                TransactionType = "Venda",
                Price = "650000",
                City = "São Paulo",
                State = "SP"
            };

        var result =
            normalizer.Normalize(
                rawProperty);

        Assert.True(
            result.IsSuccess);

        Assert.NotNull(
            result.Property);

        Assert.Null(
            result.Property!.Area);

        Assert.Null(
            result.Property.Bedrooms);

        Assert.Null(
            result.Property.Bathrooms);

        Assert.Null(
            result.Property.ParkingSpaces);

        Assert.Null(
            result.Property.Neighborhood);

        Assert.Null(
            result.Property.ExternalId);
    }

    [Fact]
    public void Normalize_ShouldNormalizeRentTransaction()
    {
        var normalizer =
            new PropertyNormalizer();

        var rawProperty =
            new RawProperty
            {
                SourceId = 1,
                PropertyType =
                    "Apartamento",
                TransactionType =
                    "Aluguel",
                Price =
                    "R$ 3.500,00",
                Area =
                    "70 m²",
                City =
                    "São Paulo",
                State =
                    "SP"
            };

        var result =
            normalizer.Normalize(
                rawProperty);

        Assert.True(
            result.IsSuccess);

        Assert.Equal(
            TransactionType.Rent,
            result.Property!.TransactionType);

        Assert.Equal(
            3_500m,
            result.Property.Price);
    }

    [Fact]
    public void Normalize_ShouldReturnFailure_WhenPriceIsInvalid()
    {
        var normalizer =
            new PropertyNormalizer();

        var rawProperty =
            new RawProperty
            {
                SourceId = 1,
                PropertyType =
                    "Apartamento",
                TransactionType =
                    "Venda",
                Price =
                    "preço sob consulta",
                City =
                    "São Paulo",
                State =
                    "SP"
            };

        var result =
            normalizer.Normalize(
                rawProperty);

        Assert.False(
            result.IsSuccess);

        Assert.Null(
            result.Property);

        Assert.Contains(
            "O preço do imóvel é inválido.",
            result.Errors);
    }

    [Fact]
    public void Normalize_ShouldReturnAllValidationErrors()
    {
        var normalizer =
            new PropertyNormalizer();

        var rawProperty =
            new RawProperty
            {
                SourceId = 0,
                PropertyType =
                    "Casa",
                TransactionType =
                    "Troca",
                Price =
                    "inválido",
                Area =
                    "-10 m²",
                Bedrooms =
                    "-2 quartos",
                City =
                    "",
                State =
                    ""
            };

        var result =
            normalizer.Normalize(
                rawProperty);

        Assert.False(
            result.IsSuccess);

        Assert.Null(
            result.Property);

        Assert.Contains(
            "A origem do imóvel é obrigatória.",
            result.Errors);

        Assert.Contains(
            "O preço do imóvel é inválido.",
            result.Errors);

        Assert.Contains(
            "A área do imóvel é inválida.",
            result.Errors);

        Assert.Contains(
            "A cidade é obrigatória.",
            result.Errors);

        Assert.Contains(
            "O estado é obrigatório.",
            result.Errors);

        Assert.Contains(
            "O tipo do imóvel é inválido.",
            result.Errors);

        Assert.Contains(
            "O tipo da transação é inválido.",
            result.Errors);

        Assert.Contains(
            "A quantidade de quartos é inválida.",
            result.Errors);
    }

    [Theory]
    [InlineData("Venda", TransactionType.Sale)]
    [InlineData("venda", TransactionType.Sale)]
    [InlineData("Sale", TransactionType.Sale)]
    [InlineData("Aluguel", TransactionType.Rent)]
    [InlineData("aluguel", TransactionType.Rent)]
    [InlineData("Locação", TransactionType.Rent)]
    [InlineData("Rent", TransactionType.Rent)]
    public void Normalize_ShouldRecognizeTransactionTypes(
        string rawTransactionType,
        TransactionType expectedTransactionType)
    {
        var normalizer =
            new PropertyNormalizer();

        var rawProperty =
            new RawProperty
            {
                SourceId = 1,
                PropertyType =
                    "Apartamento",
                TransactionType =
                    rawTransactionType,
                Price =
                    "500000",
                City =
                    "São Paulo",
                State =
                    "SP"
            };

        var result =
            normalizer.Normalize(
                rawProperty);

        Assert.True(
            result.IsSuccess);

        Assert.Equal(
            expectedTransactionType,
            result.Property!.TransactionType);
    }
}