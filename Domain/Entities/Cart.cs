namespace Domain.Entities;

public class Cart
{
    public string UserEmail { get; set; }
    public List<CartItem> Items { get; set; } = new();
}