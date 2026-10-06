using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Tests;

public class CollectionRunTests
{
    [Fact]
    public void Constructor_ShouldCreateRunningCollection()
    {
        var startedAt =
            new DateTime(
                2026,
                10,
                6,
                10,
                0,
                0,
                DateTimeKind.Utc);

        var collectionRun =
            new CollectionRun(
                sourceId: 1,
                type: CollectionRunType.FullSnapshot,
                startedAt: startedAt);

        Assert.Equal(
            1,
            collectionRun.SourceId);

        Assert.Equal(
            CollectionRunType.FullSnapshot,
            collectionRun.Type);

        Assert.Equal(
            CollectionRunStatus.Running,
            collectionRun.Status);

        Assert.Equal(
            startedAt,
            collectionRun.StartedAt);

        Assert.Null(
            collectionRun.FinishedAt);

        Assert.Null(
            collectionRun.FailureReason);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenSourceIdIsZero()
    {
        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new CollectionRun(
                    sourceId: 0,
                    type: CollectionRunType.FullSnapshot,
                    startedAt: DateTime.UtcNow));

        Assert.Equal(
            "sourceId",
            exception.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenTypeIsInvalid()
    {
        var invalidType =
            (CollectionRunType)999;

        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new CollectionRun(
                    sourceId: 1,
                    type: invalidType,
                    startedAt: DateTime.UtcNow));

        Assert.Equal(
            "type",
            exception.ParamName);
    }

    [Fact]
    public void Complete_ShouldCompleteRunningCollection()
    {
        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            CreateCollectionRun(
                startedAt);

        var finishedAt =
            startedAt.AddMinutes(5);

        collectionRun.Complete(
            finishedAt);

        Assert.Equal(
            CollectionRunStatus.Completed,
            collectionRun.Status);

        Assert.Equal(
            finishedAt,
            collectionRun.FinishedAt);

        Assert.Null(
            collectionRun.FailureReason);
    }

    [Fact]
    public void Complete_ShouldThrow_WhenFinishedAtIsBeforeStartedAt()
    {
        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            CreateCollectionRun(
                startedAt);

        var invalidFinishedAt =
            startedAt.AddSeconds(-1);

        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                collectionRun.Complete(
                    invalidFinishedAt));

        Assert.Equal(
            "finishedAt",
            exception.ParamName);

        Assert.Equal(
            CollectionRunStatus.Running,
            collectionRun.Status);
    }

    [Fact]
    public void Fail_ShouldMarkCollectionAsFailed()
    {
        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            CreateCollectionRun(
                startedAt);

        var finishedAt =
            startedAt.AddMinutes(2);

        collectionRun.Fail(
            finishedAt,
            "Falha ao consultar a fonte.");

        Assert.Equal(
            CollectionRunStatus.Failed,
            collectionRun.Status);

        Assert.Equal(
            finishedAt,
            collectionRun.FinishedAt);

        Assert.Equal(
            "Falha ao consultar a fonte.",
            collectionRun.FailureReason);
    }

    [Fact]
    public void Fail_ShouldThrow_WhenFailureReasonIsEmpty()
    {
        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            CreateCollectionRun(
                startedAt);

        var exception =
            Assert.Throws<ArgumentException>(() =>
                collectionRun.Fail(
                    startedAt.AddMinutes(1),
                    ""));

        Assert.Equal(
            "failureReason",
            exception.ParamName);

        Assert.Equal(
            CollectionRunStatus.Running,
            collectionRun.Status);
    }

    [Fact]
    public void Complete_ShouldThrow_WhenCollectionIsAlreadyCompleted()
    {
        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            CreateCollectionRun(
                startedAt);

        collectionRun.Complete(
            startedAt.AddMinutes(1));

        Assert.Throws<InvalidOperationException>(() =>
            collectionRun.Complete(
                startedAt.AddMinutes(2)));
    }

    [Fact]
    public void Complete_ShouldThrow_WhenCollectionHasFailed()
    {
        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            CreateCollectionRun(
                startedAt);

        collectionRun.Fail(
            startedAt.AddMinutes(1),
            "Erro de comunicação.");

        Assert.Throws<InvalidOperationException>(() =>
            collectionRun.Complete(
                startedAt.AddMinutes(2)));
    }

    [Fact]
public void CanReconcileMissingProperties_ShouldBeTrue_WhenFullSnapshotWithScopeIsCompleted()
{
    var startedAt =
        DateTime.UtcNow;

    var scope =
        new CollectionScope(
            city: "São Paulo",
            state: "SP",
            propertyType: PropertyType.Apartment,
            transactionType: TransactionType.Sale,
            neighborhood: "Tatuapé",
            bedrooms: 2);

    var collectionRun =
        new CollectionRun(
            sourceId: 1,
            type: CollectionRunType.FullSnapshot,
            startedAt: startedAt,
            scope: scope);

    collectionRun.Complete(
        startedAt.AddMinutes(5));

    Assert.True(
        collectionRun.CanReconcileMissingProperties);
}


[Fact]
public void CanReconcileMissingProperties_ShouldBeFalse_WhenScopeIsMissing()
{
    var startedAt =
        DateTime.UtcNow;

    var collectionRun =
        new CollectionRun(
            sourceId: 1,
            type: CollectionRunType.FullSnapshot,
            startedAt: startedAt);

    collectionRun.Complete(
        startedAt.AddMinutes(5));

    Assert.False(
        collectionRun.CanReconcileMissingProperties);
}

    [Fact]
    public void CanReconcileMissingProperties_ShouldBeFalse_WhenFullSnapshotIsRunning()
    {
        var collectionRun =
            CreateCollectionRun(
                DateTime.UtcNow,
                CollectionRunType.FullSnapshot);

        Assert.False(
            collectionRun.CanReconcileMissingProperties);
    }

    [Fact]
    public void CanReconcileMissingProperties_ShouldBeFalse_WhenFullSnapshotHasFailed()
    {
        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            CreateCollectionRun(
                startedAt,
                CollectionRunType.FullSnapshot);

        collectionRun.Fail(
            startedAt.AddMinutes(1),
            "Erro na coleta.");

        Assert.False(
            collectionRun.CanReconcileMissingProperties);
    }

    [Fact]
    public void CanReconcileMissingProperties_ShouldBeFalse_WhenPartialCollectionIsCompleted()
    {
        var startedAt =
            DateTime.UtcNow;

        var collectionRun =
            CreateCollectionRun(
                startedAt,
                CollectionRunType.Partial);

        collectionRun.Complete(
            startedAt.AddMinutes(1));

        Assert.False(
            collectionRun.CanReconcileMissingProperties);
    }

    [Fact]
    public void AssignId_ShouldAssignId()
    {
        var collectionRun =
            CreateCollectionRun(
                DateTime.UtcNow);

        collectionRun.AssignId(10);

        Assert.Equal(
            10,
            collectionRun.Id);
    }

    [Fact]
    public void AssignId_ShouldThrow_WhenIdIsZero()
    {
        var collectionRun =
            CreateCollectionRun(
                DateTime.UtcNow);

        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                collectionRun.AssignId(0));

        Assert.Equal(
            "id",
            exception.ParamName);
    }

    [Fact]
    public void AssignId_ShouldThrow_WhenCollectionAlreadyHasId()
    {
        var collectionRun =
            CreateCollectionRun(
                DateTime.UtcNow);

        collectionRun.AssignId(10);

        Assert.Throws<InvalidOperationException>(() =>
            collectionRun.AssignId(20));

        Assert.Equal(
            10,
            collectionRun.Id);
    }

    private static CollectionRun CreateCollectionRun(
        DateTime startedAt,
        CollectionRunType type =
            CollectionRunType.FullSnapshot)
    {
        return new CollectionRun(
            sourceId: 1,
            type: type,
            startedAt: startedAt);
    }
}