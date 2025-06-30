namespace Domain.Events.Cart;

public class CartItemAddedEvent
{
    public string UserEmail { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}