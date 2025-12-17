namespace Application.Dtos.Categories;

public class CategoryCreateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

