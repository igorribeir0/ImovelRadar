using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;
using ImovelRadar.Infrastructure.Repositories;

namespace ImovelRadar.Infrastructure.Tests;

public class InMemorySourceRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnDemoSource()
    {
        var repository =
            new InMemorySourceRepository();

        var sources =
            await repository.GetAllAsync();

        var source =
            Assert.Single(sources);

        Assert.Equal(
            1,
            source.Id);

        Assert.Equal(
            "Fonte Demo",
            source.Name);

        Assert.Equal(
            SourceType.Api,
            source.Type);

        Assert.True(
            source.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSource_WhenSourceExists()
    {
        var repository =
            new InMemorySourceRepository();

        var source =
            await repository.GetByIdAsync(1);

        Assert.NotNull(source);

        Assert.Equal(
            "Fonte Demo",
            source!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenSourceDoesNotExist()
    {
        var repository =
            new InMemorySourceRepository();

        var source =
            await repository.GetByIdAsync(999);

        Assert.Null(source);
    }

    [Fact]
    public async Task AddAsync_ShouldAssignIdAndAddSource()
    {
        var repository =
            new InMemorySourceRepository();

        var source =
            new Source(
                name: "Importação CSV",
                type: SourceType.Csv);

        await repository.AddAsync(
            source);

        Assert.Equal(
            2,
            source.Id);

        var sources =
            await repository.GetAllAsync();

        Assert.Equal(
            2,
            sources.Count);

        Assert.Contains(
            sources,
            item =>
                item.Id == 2 &&
                item.Name == "Importação CSV");
    }

    [Fact]
    public async Task GetActiveAsync_ShouldReturnOnlyActiveSources()
    {
        var repository =
            new InMemorySourceRepository();

        var inactiveSource =
            new Source(
                name: "Fonte Inativa",
                type: SourceType.Manual);

        await repository.AddAsync(
            inactiveSource);

        inactiveSource.Deactivate();

        await repository.UpdateAsync(
            inactiveSource);

        var activeSources =
            await repository.GetActiveAsync();

        Assert.Single(
            activeSources);

        Assert.All(
            activeSources,
            source =>
                Assert.True(source.IsActive));

        Assert.DoesNotContain(
            activeSources,
            source =>
                source.Id == inactiveSource.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistSourceChanges()
    {
        var repository =
            new InMemorySourceRepository();

        var source =
            await repository.GetByIdAsync(1);

        Assert.NotNull(source);

        source!.Deactivate();

        await repository.UpdateAsync(
            source);

        var updatedSource =
            await repository.GetByIdAsync(1);

        Assert.NotNull(
            updatedSource);

        Assert.False(
            updatedSource!.IsActive);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenSourceDoesNotExist()
    {
        var repository =
            new InMemorySourceRepository();

        var source =
            new Source(
                name: "Fonte inexistente",
                type: SourceType.Api);

        source.AssignId(999);

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await repository.UpdateAsync(
                    source));
    }
}