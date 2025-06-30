using Application.Interfaces.Services;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductResultDto> CreateAsync(ProductCreateDto dto)
    {
        var entity = new ProductEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            Value = dto.Value,
            ImageUrl = dto.ImageUrl
        };

        var result = await _repository.AddAsync(entity);
        return ToResultDto(result);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProductResultDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(ToResultDto);
    }

    public async Task<ProductResultDto> GetByIdAsync(string id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : ToResultDto(product);
    }

    public async Task<ProductResultDto> UpdateAsync(string id, ProductUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Value = dto.Value;
        entity.ImageUrl = dto.ImageUrl;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity);
        return ToResultDto(result);
    }
    
    public async Task<ProductResultDto> UpdatePartialTask(string id, ProductUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;

        // Update only the fields that are provided in the DTO
        if (!string.IsNullOrEmpty(dto.Name)) entity.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Description)) entity.Description = dto.Description;
        if (dto.Value > 0) entity.Value = dto.Value; // Assuming Value should be positive
        if (!string.IsNullOrEmpty(dto.ImageUrl)) entity.ImageUrl = dto.ImageUrl;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity);
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