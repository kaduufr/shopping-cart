
namespace Infrastructure.Repositories;

public class RedisCartRepository : ICartRepository
{
    private readonly IDatabase _redis;

    public RedisCartRepository(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task<Cart> GetCartAsync(string userEmail)
    {
        var data = await _redis.StringGetAsync(userEmail);
        if (data.IsNullOrEmpty)
        {
            return new Cart { UserEmail = userEmail };
        }
        return JsonSerializer.Deserialize<Cart>(data);
    }

    public async Task<Cart> UpdateCartAsync(Cart cart)
    {
        var serializedCart = JsonSerializer.Serialize(cart);
        await _redis.StringSetAsync(cart.UserEmail, serializedCart);
        return await GetCartAsync(cart.UserEmail);
    }

    public async Task<bool> DeleteCartAsync(string userEmail)
    {
        return await _redis.KeyDeleteAsync(userEmail);
    }
}