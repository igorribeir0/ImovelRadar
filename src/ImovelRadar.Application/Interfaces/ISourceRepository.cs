using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Application.Interfaces;

public interface ISourceRepository
{
    Task<IReadOnlyList<Source>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Source>> GetActiveAsync(
        CancellationToken cancellationToken = default);

    Task<Source?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Source source,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Source source,
        CancellationToken cancellationToken = default);
}