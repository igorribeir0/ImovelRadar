using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Domain.Entities;

public class CollectionRun
{
    public CollectionRun(
        int sourceId,
        CollectionRunType type,
        DateTime startedAt,
        CollectionScope? scope = null)
    {
        if (sourceId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sourceId),
                "A fonte da coleta é obrigatória.");
        }

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                "O tipo da coleta é inválido.");
        }

        SourceId = sourceId;
        Type = type;
        StartedAt = startedAt;
        Scope = scope;
        Status = CollectionRunStatus.Running;
    }

    public long Id { get; private set; }

    public int SourceId { get; private set; }

    public CollectionRunType Type { get; private set; }

    public CollectionRunStatus Status { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime? FinishedAt { get; private set; }

    public string? FailureReason { get; private set; }

    public CollectionScope? Scope { get; private set; }

    public bool CanReconcileMissingProperties =>
        Type == CollectionRunType.FullSnapshot &&
        Status == CollectionRunStatus.Completed &&
        Scope is not null;

    public void Complete(
        DateTime finishedAt)
    {
        EnsureRunning();

        if (finishedAt < StartedAt)
        {
            throw new ArgumentOutOfRangeException(
                nameof(finishedAt),
                "A data de término não pode ser anterior ao início da coleta.");
        }

        Status = CollectionRunStatus.Completed;
        FinishedAt = finishedAt;
        FailureReason = null;
    }

    public void Fail(
        DateTime finishedAt,
        string failureReason)
    {
        EnsureRunning();

        if (finishedAt < StartedAt)
        {
            throw new ArgumentOutOfRangeException(
                nameof(finishedAt),
                "A data de término não pode ser anterior ao início da coleta.");
        }

        if (string.IsNullOrWhiteSpace(failureReason))
        {
            throw new ArgumentException(
                "O motivo da falha é obrigatório.",
                nameof(failureReason));
        }

        Status = CollectionRunStatus.Failed;
        FinishedAt = finishedAt;
        FailureReason = failureReason.Trim();
    }

    public void AssignId(
        long id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(id),
                "O identificador da coleta deve ser maior que zero.");
        }

        if (Id != 0)
        {
            throw new InvalidOperationException(
                "A coleta já possui um identificador.");
        }

        Id = id;
    }

    private void EnsureRunning()
    {
        if (Status != CollectionRunStatus.Running)
        {
            throw new InvalidOperationException(
                "A coleta já foi finalizada.");
        }
    }
}