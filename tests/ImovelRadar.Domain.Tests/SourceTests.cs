using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Tests;

public class SourceTests
{
    [Fact]
    public void Constructor_ShouldCreateActiveSource()
    {
        var source =
            new Source(
                name: "Parceiro Imobiliário",
                type: SourceType.Api,
                baseUrl: "https://example.com");

        Assert.Equal(
            "Parceiro Imobiliário",
            source.Name);

        Assert.Equal(
            SourceType.Api,
            source.Type);

        Assert.Equal(
            "https://example.com",
            source.BaseUrl);

        Assert.True(
            source.IsActive);

        Assert.NotEqual(
            default,
            source.CreatedAt);

        Assert.NotEqual(
            default,
            source.UpdatedAt);
    }

    [Fact]
    public void Constructor_ShouldTrimNameAndBaseUrl()
    {
        var source =
            new Source(
                name: "  Parceiro Imobiliário  ",
                type: SourceType.Api,
                baseUrl: "  https://example.com  ");

        Assert.Equal(
            "Parceiro Imobiliário",
            source.Name);

        Assert.Equal(
            "https://example.com",
            source.BaseUrl);
    }

    [Fact]
    public void Constructor_ShouldAllowNullBaseUrl()
    {
        var source =
            new Source(
                name: "Importação Manual",
                type: SourceType.Manual);

        Assert.Null(
            source.BaseUrl);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenNameIsEmpty()
    {
        var exception =
            Assert.Throws<ArgumentException>(() =>
                new Source(
                    name: "",
                    type: SourceType.Api));

        Assert.Equal(
            "name",
            exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenTypeIsInvalid()
    {
        var invalidType =
            (SourceType)999;

        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Source(
                    name: "Fonte inválida",
                    type: invalidType));

        Assert.Equal(
            "type",
            exception.ParamName);
    }

    [Fact]
    public void Deactivate_ShouldDeactivateSource()
    {
        var source =
            CreateValidSource();

        source.Deactivate();

        Assert.False(
            source.IsActive);
    }

    [Fact]
    public void Activate_ShouldActivateInactiveSource()
    {
        var source =
            CreateValidSource();

        source.Deactivate();

        Assert.False(
            source.IsActive);

        source.Activate();

        Assert.True(
            source.IsActive);
    }

    [Fact]
    public void AssignId_ShouldAssignId()
    {
        var source =
            CreateValidSource();

        source.AssignId(10);

        Assert.Equal(
            10,
            source.Id);
    }

    [Fact]
    public void AssignId_ShouldThrow_WhenIdIsZero()
    {
        var source =
            CreateValidSource();

        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                source.AssignId(0));

        Assert.Equal(
            "id",
            exception.ParamName);
    }

    [Fact]
    public void AssignId_ShouldThrow_WhenSourceAlreadyHasId()
    {
        var source =
            CreateValidSource();

        source.AssignId(10);

        Assert.Throws<InvalidOperationException>(() =>
            source.AssignId(20));

        Assert.Equal(
            10,
            source.Id);
    }

    private static Source CreateValidSource()
    {
        return new Source(
            name: "Parceiro Imobiliário",
            type: SourceType.Api,
            baseUrl: "https://example.com");
    }
}