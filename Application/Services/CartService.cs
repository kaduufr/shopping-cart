
namespace Application.Services;

public class CartService : ICartService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ICartRepository _cartRepository;
    private readonly ILogger<CartService> _logger;

    public CartService(IEventPublisher eventPublisher, ICartRepository cartRepository, ILogger<CartService> logger)
    {
        _eventPublisher = eventPublisher;
        _cartRepository = cartRepository;
        _logger = logger;
    }

    public async Task AddItemToCartAsync(string userEmail, AddItemToCartDto dto)
    {
        try
        {
            _logger.LogInformation("Publishing event to add item to cart");
            var itemAddedEvent = new CartItemAddedEvent { UserEmail = userEmail, ProductId = dto.ProductId, Quantity = dto.Quantity };
            await _eventPublisher.PublishAsync("cart-events", new { EventType = "ItemAdded", Data = itemAddedEvent });
            _logger.LogInformation("Event published successfully for user {UserEmail}", userEmail);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding item to cart for user {UserEmail}", userEmail);
            throw new BadHttpRequestException("Failed to add item to cart", e);
        }
    }

    public async Task RemoveItemFromCartAsync(string userEmail, RemoveItemFromCartDto dto)
    {
        try
        {
            _logger.LogInformation("Publishing event to remove item from cart");
            var itemRemovedEvent = new CartItemRemovedEvent { UserEmail = userEmail, ProductId = dto.ProductId };
            await _eventPublisher.PublishAsync("cart-events", new { EventType = "ItemRemoved", Data = itemRemovedEvent });
            _logger.LogInformation("Event published successfully for user {UserEmail}", userEmail);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing item from cart for user {UserEmail}", userEmail);
            throw new BadHttpRequestException("Failed to remove item from cart", e);
        }

    }

    public async Task<CartDto> GetCartAsync(string userEmail)
    {
        _logger.LogInformation("Retrieving cart for user {UserEmail}", userEmail);
        var cart = await _cartRepository.GetCartAsync(userEmail);
        return new CartDto
        {
            Items = cart.Items.Select(i => new CartItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList()
        };
    }
}