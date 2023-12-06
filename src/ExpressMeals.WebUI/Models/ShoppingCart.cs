using ExpressMeals.Contracts.ViewModels;

namespace ExpressMeals.WebUI.Models;

public class ShoppingCart
{
    public int MealId { get; set; }
    public MealVm Meal { get; set; } = null!;
    public int Count { get; set; }
}