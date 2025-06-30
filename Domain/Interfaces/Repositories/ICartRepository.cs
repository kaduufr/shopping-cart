namespace Domain.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart> GetCartAsync(string userEmail);
    Task<Cart> UpdateCartAsync(Cart cart);
    Task<bool> DeleteCartAsync(string userEmail);
}