namespace Application.Dtos.Cart;

public class AddItemToCartDto
{
    public string ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}