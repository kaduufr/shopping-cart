using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProductResultDto> CreateAsync(ProductCreateDto dto)
    {
        _logger.LogInformation("Creating product with name {Name}", dto.Name);
        var entity = new ProductEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            Value = dto.Value,
            ImageUrl = dto.ImageUrl
        };

        var result = await _repository.AddAsync(entity);
        _logger.LogInformation("Product {Name} created with id {Id}", result.Name, result.Id);
        return ToResultDto(result);
    }

    public async Task InsertProductsBulkAsync(List<ProductCreateDto> products)
    {
        _logger.LogInformation("Inserting {Count} products in bulk", products.Count);
        var entities = products.Select(dto => new ProductEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            Value = dto.Value,
            ImageUrl = dto.ImageUrl
        }).ToList();

        await _repository.InsertProductsBulkAsync(entities);
        _logger.LogInformation("{Count} products inserted in bulk", entities.Count);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        _logger.LogInformation("Deleting product with id {Id}", id);
        var result = await _repository.DeleteAsync(id);
        if (result)
            _logger.LogInformation("Product with id {Id} deleted", id);
        else
            _logger.LogWarning("Product with id {Id} not found", id);
        return result;
    }

    public async Task<IEnumerable<ProductResultDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all products");
        var products = await _repository.GetAllAsync();
        _logger.LogInformation("Found {Count} products", products.Count());
        return products.Select(ToResultDto);
    }

    public async Task<ProductResultDto?> GetByIdAsync(string id)
    {
        _logger.LogInformation("Getting product by id {Id}", id);
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product with id {Id} not found", id);
            return null;
        }
        _logger.LogInformation("Found product with id {Id}", id);
        return ToResultDto(product);
    }

    public async Task<ProductResultDto?> UpdateAsync(string id, ProductUpdateDto dto)
    {
        _logger.LogInformation("Updating product with id {Id}", id);
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Product with id {Id} not found", id);
            return null;
        }

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Value = dto.Value;
        entity.ImageUrl = dto.ImageUrl;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity);
        _logger.LogInformation("Product with id {Id} updated", id);
        return ToResultDto(result);
    }

    public async Task<ProductResultDto?> UpdatePartialAsync(string id, [FromBody]ProductUpdateDto? patchDto)
    {
        _logger.LogInformation("Partially updating product with id {Id}", id);
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Product with id {Id} not found", id);
            return null;
        }

        var dto = new ProductUpdateDto
        {
            Name = patchDto?.Name ?? entity.Name,
            Description = patchDto?.Description ?? entity.Description,
            Value = patchDto?.Value ?? entity.Value,
            ImageUrl = patchDto?.ImageUrl ?? entity.ImageUrl,
            IsActive = patchDto?.IsActive ?? entity.IsActive
        };

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Value = dto.Value;
        entity.ImageUrl = dto.ImageUrl;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity);
        _logger.LogInformation("Product with id {Id} partially updated", id);
        return ToResultDto(result);
    }

    private ProductResultDto ToResultDto(ProductEntity entity)
    {
        return new ProductResultDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Value = entity.Value,
            ImageUrl = entity.ImageUrl,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}