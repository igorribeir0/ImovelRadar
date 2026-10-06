using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Interfaces;
using ImovelRadar.Domain.Entities;

namespace ImovelRadar.Application.Services;

public class PropertyQueryService
{
    private const int MaxPageSize = 100;

    private readonly IPropertyRepository _propertyRepository;

    public PropertyQueryService(
        IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PagedResponse<PropertyResponse>> GetAllAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        ValidatePagination(
            page,
            pageSize);

        var properties =
            await _propertyRepository.GetAllAsync(
                cancellationToken);

        return CreatePagedResponse(
            properties,
            page,
            pageSize);
    }

    public async Task<PagedResponse<PropertyResponse>> SearchAsync(
        PropertySearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateSearchRequest(request);

        var properties =
            await _propertyRepository.SearchAsync(
                request,
                cancellationToken);

        return CreatePagedResponse(
            properties,
            request.Page,
            request.PageSize);
    }

    public async Task<PropertyResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(id),
                "O identificador do imóvel deve ser maior que zero.");
        }

        var property =
            await _propertyRepository.GetByIdAsync(
                id,
                cancellationToken);

        if (property is null)
        {
            throw new InvalidOperationException(
                "Imóvel não encontrado.");
        }

        return MapToResponse(property);
    }

    private static PagedResponse<PropertyResponse>
        CreatePagedResponse(
            IEnumerable<Property> properties,
            int page,
            int pageSize)
    {
        var propertyList =
            properties.ToList();

        var totalItems =
            propertyList.Count;

        var totalPages =
            totalItems == 0
                ? 0
                : (int)Math.Ceiling(
                    totalItems /
                    (double)pageSize);

        var items =
            propertyList
                .Skip(
                    (page - 1) *
                    pageSize)
                .Take(
                    pageSize)
                .Select(MapToResponse)
                .ToList();

        return new PagedResponse<PropertyResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    private static void ValidateSearchRequest(
        PropertySearchRequest request)
    {
        if (request.MinPrice is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.MinPrice),
                "O preço mínimo não pode ser negativo.");
        }

        if (request.MaxPrice is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.MaxPrice),
                "O preço máximo não pode ser negativo.");
        }

        if (request.MinPrice.HasValue &&
            request.MaxPrice.HasValue &&
            request.MinPrice > request.MaxPrice)
        {
            throw new ArgumentException(
                "O preço mínimo não pode ser maior que o preço máximo.");
        }

        if (request.MinArea is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.MinArea),
                "A área mínima deve ser maior que zero.");
        }

        if (request.MaxArea is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.MaxArea),
                "A área máxima deve ser maior que zero.");
        }

        if (request.MinArea.HasValue &&
            request.MaxArea.HasValue &&
            request.MinArea > request.MaxArea)
        {
            throw new ArgumentException(
                "A área mínima não pode ser maior que a área máxima.");
        }

        if (request.Bedrooms is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.Bedrooms),
                "A quantidade de quartos não pode ser negativa.");
        }

        ValidatePagination(
            request.Page,
            request.PageSize);
    }

    private static void ValidatePagination(
        int page,
        int pageSize)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(page),
                "A página deve ser maior ou igual a 1.");
        }

        if (pageSize < 1 ||
            pageSize > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                $"O tamanho da página deve estar entre 1 e {MaxPageSize}.");
        }
    }

    private static PropertyResponse MapToResponse(
        Property property)
    {
        return new PropertyResponse
        {
            Id = property.Id,
            Title = property.Title,
            PropertyType =
                property.PropertyType.ToString(),
            TransactionType =
                property.TransactionType.ToString(),
            Price = property.Price,
            Area = property.Area,
            PricePerSquareMeter =
                property.GetPricePerSquareMeter(),
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            ParkingSpaces = property.ParkingSpaces,
            Neighborhood = property.Neighborhood,
            City = property.City,
            State = property.State,
            SourceUrl = property.SourceUrl,
            FirstSeenAt = property.FirstSeenAt,
            LastSeenAt = property.LastSeenAt
        };
    }
}