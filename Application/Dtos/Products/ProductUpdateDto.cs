namespace Application.Dtos.Products;

public class ProductUpdateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}