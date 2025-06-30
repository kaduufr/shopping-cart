namespace Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductResultDto> CreateAsync(ProductCreateDto dto);
        Task<ProductResultDto> UpdateAsync(string id, ProductUpdateDto dto);
        Task<bool> DeleteAsync(string id);
        Task<ProductResultDto> GetByIdAsync(string id);
        Task<IEnumerable<ProductResultDto>> GetAllAsync();
    }
}
