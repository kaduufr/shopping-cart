using Application.Dtos.Categories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CategoryResultDto> CreateAsync(CategoryCreateDto dto)
    {
        _logger.LogInformation("Creating category with name {Name}", dto.Name);
        var entity = new CategoryEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive ?? true
        };

        var result = await _repository.AddAsync(entity);
        _logger.LogInformation("Category {Name} created with id {Id}", result.Name, result.Id);
        return ToResultDto(result);
    }

    public async Task<CategoryResultDto?> GetByIdAsync(string id)
    {
        _logger.LogInformation("Getting category by id {Id}", id);
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Category with id {Id} not found", id);
            return null;
        }
        _logger.LogInformation("Found category with id {Id}", id);
        return ToResultDto(category);
    }

    public async Task<IEnumerable<CategoryResultDto>> GetAllAsync()
    {
        _logger.LogInformation("Getting all categories");
        var categories = await _repository.GetAllAsync();
        _logger.LogInformation("Found {Count} categories", categories.Count());
        return categories.Select(ToResultDto);
    }

    public async Task<CategoryResultDto?> UpdateAsync(string id, CategoryUpdateDto dto)
    {
        _logger.LogInformation("Updating category with id {Id}", id);
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Category with id {Id} not found", id);
            return null;
        }

        entity.Name = dto.Name ?? entity.Name;
        entity.Description = dto.Description ?? entity.Description;
        entity.IsActive = dto.IsActive ?? entity.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity);
        _logger.LogInformation("Category with id {Id} updated", id);
        return ToResultDto(result);
    }

    public async Task<CategoryResultDto?> UpdatePartialAsync(string id, CategoryUpdateDto dto)
    {
        _logger.LogInformation("Partially updating category with id {Id}", id);
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Category with id {Id} not found", id);
            return null;
        }

        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(entity);
        _logger.LogInformation("Category with id {Id} partially updated", id);
        return ToResultDto(result);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        _logger.LogInformation("Deleting category with id {Id}", id);
        var result = await _repository.DeleteAsync(id);
        if (result)
            _logger.LogInformation("Category with id {Id} deleted", id);
        else
            _logger.LogWarning("Category with id {Id} not found", id);
        return result;
    }

    private CategoryResultDto ToResultDto(CategoryEntity entity)
    {
        return new CategoryResultDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}