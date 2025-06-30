namespace Application.Dtos.Cart;

public class CartDto
{
    public string UserEmail { get; set; }
    public List<CartItemDto> Items { get; set; }
}