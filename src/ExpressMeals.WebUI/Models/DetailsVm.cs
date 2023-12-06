using System.ComponentModel.DataAnnotations;
using ExpressMeals.Contracts.ViewModels;

namespace ExpressMeals.WebUI.Models;

public class DetailsVm
{
    public DetailsVm()
    {
        Meal = new();
        Count = 1;

    }

    [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than 0")]
    public int Count { get; set; }
    [Required]
    public int MealId { get; set; }
    public MealVm Meal { get; set; }
}