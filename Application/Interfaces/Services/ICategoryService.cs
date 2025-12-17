namespace Application.Interfaces.Services;

using Application.Dtos.Categories;

public interface ICategoryService
{
    Task<CategoryResultDto> CreateAsync(CategoryCreateDto dto);
    Task<IEnumerable<CategoryResultDto>> GetAllAsync();
    Task<CategoryResultDto?> GetByIdAsync(string id);
    Task<CategoryResultDto?> UpdateAsync(string id, CategoryUpdateDto dto);
    Task<CategoryResultDto?> UpdatePartialAsync(string id, CategoryUpdateDto dto);
    Task<bool> DeleteAsync(string id);
}