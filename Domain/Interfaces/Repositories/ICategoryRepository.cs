using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<CategoryEntity> AddAsync(CategoryEntity category);
    Task<CategoryEntity> UpdateAsync(CategoryEntity category);
    Task<bool> DeleteAsync(string id);
    Task<CategoryEntity?> GetByIdAsync(string id);
    Task<IEnumerable<CategoryEntity>> GetAllAsync();
}
