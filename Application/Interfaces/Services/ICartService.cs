
namespace Application.Interfaces.Services;

public interface ICartService
{
    Task AddItemToCartAsync(string userEmail, AddItemToCartDto dto);
    Task RemoveItemFromCartAsync(string userEmail, RemoveItemFromCartDto dto);
    Task<CartDto> GetCartAsync(string userEmail);
}