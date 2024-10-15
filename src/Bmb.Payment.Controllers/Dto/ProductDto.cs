namespace Bmb.Payment.Controllers.Dto;

public class ProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ProductCategoryDto Category { get; set; }

    public decimal Price { get; set; }

    public string[] Images { get; set; } = Array.Empty<string>();

}

public enum ProductCategoryDto
{
    Meal = 0,
    Sides,
    Drink,
    Dessert
}
