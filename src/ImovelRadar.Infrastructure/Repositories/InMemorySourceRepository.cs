using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;
using ImovelRadar.Domain.Enums;

namespace ImovelRadar.Infrastructure.Repositories;

public class InMemorySourceRepository
    : ISourceRepository
{
    private readonly List<Source> _sources;

    public InMemorySourceRepository()
    {
        _sources =
        [
            CreateSource(
                id: 1,
                name: "Fonte Demo",
                type: SourceType.Api,
                baseUrl: "https://example.com")
        ];
    }

    public Task<IReadOnlyList<Source>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Source> result =
            _sources.ToList();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<Source>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Source> result =
            _sources
                .Where(source =>
                    source.IsActive)
                .ToList();

        return Task.FromResult(result);
    }

    public Task<Source?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var source =
            _sources.FirstOrDefault(
                item => item.Id == id);

        return Task.FromResult(source);
    }

    public Task AddAsync(
        Source source,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.Id == 0)
        {
            var nextId =
                _sources.Count == 0
                    ? 1
                    : _sources.Max(
                        item => item.Id) + 1;

            source.AssignId(nextId);
        }

        _sources.Add(source);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Source source,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        var exists =
            _sources.Any(
                item => item.Id == source.Id);

        if (!exists)
        {
            throw new InvalidOperationException(
                $"A fonte {source.Id} não foi encontrada.");
        }

        // Como o repositório trabalha com as mesmas
        // referências dos objetos em memória,
        // nenhuma substituição é necessária aqui.
        return Task.CompletedTask;
    }

    private static Source CreateSource(
        int id,
        string name,
        SourceType type,
        string? baseUrl)
    {
        var source =
            new Source(
                name: name,
                type: type,
                baseUrl: baseUrl);

        source.AssignId(id);

        return source;
    }
}