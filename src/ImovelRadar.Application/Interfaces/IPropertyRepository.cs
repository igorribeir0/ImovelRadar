using ImovelRadar.Application.DTOs;
using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Application.Interfaces;

public interface IPropertyRepository
{
    Task<IReadOnlyList<Property>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> SearchAsync(
        PropertySearchRequest request,
        CancellationToken cancellationToken = default);

    Task<Property?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Property?> GetBySourceAndExternalIdAsync(
        int sourceId,
        string externalId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> GetComparablePropertiesAsync(
        Property property,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Property property,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Property property,
        CancellationToken cancellationToken = default);
}