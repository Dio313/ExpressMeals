using System.ComponentModel.DataAnnotations;

namespace ExpressMeals.Contracts.ViewModels;

public class MealVm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Price is required")]
    public double Price { get; set; }

    [Required(ErrorMessage = "Image is required")]
    public string ImageUrl { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }
}

