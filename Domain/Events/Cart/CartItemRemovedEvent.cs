namespace Domain.Events.Cart;

public class CartItemRemovedEvent
{
    public string UserEmail { get; set; }
    public string ProductId { get; set; }
}