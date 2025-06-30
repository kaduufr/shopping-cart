
namespace Application.Services;

public class CartService : ICartService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ICartRepository _cartRepository;

    public CartService(IEventPublisher eventPublisher, ICartRepository cartRepository)
    {
        _eventPublisher = eventPublisher;
        _cartRepository = cartRepository;
    }

    public async Task AddItemToCartAsync(string userEmail, AddItemToCartDto dto)
    {
        var itemAddedEvent = new CartItemAddedEvent { UserEmail = userEmail, ProductId = dto.ProductId, Quantity = dto.Quantity };
        await _eventPublisher.PublishAsync("cart-events", new { EventType = "ItemAdded", Data = itemAddedEvent });
    }

    public async Task RemoveItemFromCartAsync(string userEmail, RemoveItemFromCartDto dto)
    {
        var itemRemovedEvent = new CartItemRemovedEvent { UserEmail = userEmail, ProductId = dto.ProductId };
        await _eventPublisher.PublishAsync("cart-events", new { EventType = "ItemRemoved", Data = itemRemovedEvent });
    }

    public async Task<CartDto> GetCartAsync(string userEmail)
    {
        var cart = await _cartRepository.GetCartAsync(userEmail);
        return new CartDto
        {
            UserEmail = cart.UserEmail,
            Items = cart.Items.Select(i => new CartItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList()
        };
    }
}