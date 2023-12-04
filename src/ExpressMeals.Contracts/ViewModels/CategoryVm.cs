using System.ComponentModel.DataAnnotations;

namespace ExpressMeals.Contracts.ViewModels;

public class CategoryVm
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Category Name is required")]
    public string Name { get; set; }
    public string Icon { get; set; }
}

public class MealCategoryVm
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<MealVm> Meals { get; set; } = new();
}

