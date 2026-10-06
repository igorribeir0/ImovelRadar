using ImovelRadar.Application.Collection;
using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Application.Tests;

public class PropertyReconciliationServiceTests
{
    [Fact]
    public async Task ReconcileAsync_ShouldMarkMissingPropertyAsNotObserved()
    {
        var propertyA =
            CreateProperty(
                id: 1,
                externalId: "A",
                neighborhood: "Tatuapé");

        var propertyB =
            CreateProperty(
                id: 2,
                externalId: "B",
                neighborhood: "Tatuapé");

        var repository =
            new FakePropertyRepository(
                propertyA,
                propertyB);

        var service =
            new PropertyReconciliationService(
                repository);

        var collectionRun =
            CreateCompletedFullSnapshot();

        var result =
            await service.ReconcileAsync(
                collectionRun,
                new[] { "A" });

        Assert.Equal(
            PropertyObservationStatus.Active,
            propertyA.ObservationStatus);

        Assert.Equal(
            PropertyObservationStatus.NotObserved,
            propertyB.ObservationStatus);

        Assert.Equal(
            2,
            result.Evaluated);

        Assert.Equal(
            1,
            result.MarkedAsNotObserved);

        Assert.Equal(
            1,
            repository.UpdateCount);
    }

    [Fact]
    public async Task ReconcileAsync_ShouldNotTouchPropertyOutsideScope()
    {
        var tatuape =
            CreateProperty(
                id: 1,
                externalId: "A",
                neighborhood: "Tatuapé");

        var mooca =
            CreateProperty(
                id: 2,
                externalId: "B",
                neighborhood: "Mooca");

        var repository =
            new FakePropertyRepository(
                tatuape,
                mooca);

        var service =
            new PropertyReconciliationService(
                repository);

        var collectionRun =
            CreateCompletedFullSnapshot();

        var result =
            await service.ReconcileAsync(
                collectionRun,
                Array.Empty<string>());

        Assert.Equal(
            PropertyObservationStatus.NotObserved,
            tatuape.ObservationStatus);

        Assert.Equal(
            PropertyObservationStatus.Active,
            mooca.ObservationStatus);

        Assert.Equal(
            1,
            result.Evaluated);
    }

    [Fact]
    public async Task ReconcileAsync_ShouldNotTouchPropertyFromAnotherSource()
    {
        var sourceOne =
            CreateProperty(
                id: 1,
                externalId: "A",
                neighborhood: "Tatuapé",
                sourceId: 1);

        var sourceTwo =
            CreateProperty(
                id: 2,
                externalId: "B",
                neighborhood: "Tatuapé",
                sourceId: 2);

        var repository =
            new FakePropertyRepository(
                sourceOne,
                sourceTwo);

        var service =
            new PropertyReconciliationService(
                repository);

        var collectionRun =
            CreateCompletedFullSnapshot();

        await service.ReconcileAsync(
            collectionRun,
            Array.Empty<string>());

        Assert.Equal(
            PropertyObservationStatus.NotObserved,
            sourceOne.ObservationStatus);

        Assert.Equal(
            PropertyObservationStatus.Active,
            sourceTwo.ObservationStatus);
    }

    [Fact]
    public async Task ReconcileAsync_ShouldThrow_WhenCollectionIsPartial()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyReconciliationService(
                repository);

        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            new CollectionRun(
                sourceId: 1,
                type: CollectionRunType.Partial,
                startedAt: startedAt,
                scope: CreateScope());

        collectionRun.Complete(
            startedAt.AddMinutes(1));

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await service.ReconcileAsync(
                    collectionRun,
                    Array.Empty<string>()));
    }

    [Fact]
    public async Task ReconcileAsync_ShouldThrow_WhenCollectionHasFailed()
    {
        var repository =
            new FakePropertyRepository();

        var service =
            new PropertyReconciliationService(
                repository);

        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            new CollectionRun(
                sourceId: 1,
                type: CollectionRunType.FullSnapshot,
                startedAt: startedAt,
                scope: CreateScope());

        collectionRun.Fail(
            startedAt.AddMinutes(1),
            "Falha na fonte.");

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
                await service.ReconcileAsync(
                    collectionRun,
                    Array.Empty<string>()));
    }

    [Fact]
    public async Task ReconcileAsync_ShouldNotUpdatePropertyAlreadyNotObserved()
    {
        var property =
            CreateProperty(
                id: 1,
                externalId: "A",
                neighborhood: "Tatuapé");

        property.MarkAsNotObserved();

        var repository =
            new FakePropertyRepository(
                property);

        var service =
            new PropertyReconciliationService(
                repository);

        var result =
            await service.ReconcileAsync(
                CreateCompletedFullSnapshot(),
                Array.Empty<string>());

        Assert.Equal(
            PropertyObservationStatus.NotObserved,
            property.ObservationStatus);

        Assert.Equal(
            0,
            repository.UpdateCount);

        Assert.Equal(
            0,
            result.MarkedAsNotObserved);
    }

    private static CollectionRun
        CreateCompletedFullSnapshot()
    {
        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            new CollectionRun(
                sourceId: 1,
                type: CollectionRunType.FullSnapshot,
                startedAt: startedAt,
                scope: CreateScope());

        collectionRun.Complete(
            startedAt.AddMinutes(1));

        return collectionRun;
    }

    private static CollectionScope CreateScope()
    {
        return new CollectionScope(
            city: "São Paulo",
            state: "SP",
            propertyType: PropertyType.Apartment,
            transactionType: TransactionType.Sale,
            neighborhood: "Tatuapé",
            bedrooms: 2);
    }

    private static Property CreateProperty(
        int id,
        string externalId,
        string neighborhood,
        int sourceId = 1)
    {
        var property =
            new Property(
                title: "Apartamento",
                propertyType: PropertyType.Apartment,
                transactionType: TransactionType.Sale,
                price: 650_000m,
                area: 80m,
                bedrooms: 2,
                bathrooms: 2,
                parkingSpaces: 1,
                city: "São Paulo",
                state: "SP",
                sourceId: sourceId,
                neighborhood: neighborhood,
                externalId: externalId);

        property.AssignId(id);

        return property;
    }

    private sealed class FakePropertyRepository
        : IPropertyRepository
    {
        private readonly List<Property>
            _properties;

        public FakePropertyRepository(
            params Property[] properties)
        {
            _properties =
                properties.ToList();
        }

        public int UpdateCount { get; private set; }

        public Task<IReadOnlyList<Property>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Property> result =
                _properties.ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<Property>> SearchAsync(
            PropertySearchRequest request,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Property> result =
                _properties.ToList();

            return Task.FromResult(result);
        }

        public Task<Property?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _properties.FirstOrDefault(
                    property =>
                        property.Id == id));
        }

        public Task<Property?> GetBySourceAndExternalIdAsync(
            int sourceId,
            string externalId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _properties.FirstOrDefault(
                    property =>
                        property.SourceId == sourceId &&
                        string.Equals(
                            property.ExternalId,
                            externalId,
                            StringComparison.OrdinalIgnoreCase)));
        }

        public Task<IReadOnlyList<Property>>
            GetComparablePropertiesAsync(
                Property property,
                CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Property> result =
                _properties
                    .Where(item =>
                        !ReferenceEquals(
                            item,
                            property))
                    .ToList();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            Property property,
            CancellationToken cancellationToken = default)
        {
            _properties.Add(
                property);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            Property property,
            CancellationToken cancellationToken = default)
        {
            UpdateCount++;

            return Task.CompletedTask;
        }
    }
}