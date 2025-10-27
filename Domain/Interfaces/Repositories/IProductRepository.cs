
namespace Domain.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<ProductEntity> AddAsync(ProductEntity product);
        Task<ProductEntity> UpdateAsync(ProductEntity product);
        Task<bool> DeleteAsync(string id);
        Task<ProductEntity?> GetByIdAsync(string id);
        Task<IEnumerable<ProductEntity>> GetAllAsync();
        Task InsertProductsBulkAsync(List<ProductEntity> products);
    }
}
