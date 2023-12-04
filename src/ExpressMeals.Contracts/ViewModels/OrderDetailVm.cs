using System.ComponentModel.DataAnnotations;

namespace ExpressMeals.Contracts.ViewModels;

public class OrderDetailVm
{
    public int Id { get; set; }
    [Required]
    public int OrderHeaderId { get; set; }

    [Required]
    public int MealId { get; set; }
    public MealVm Meals { get; set; }

    [Required]
    public int Count { get; set; }
    [Required]
    public double Price { get; set; }
    [Required]
    public string Size { get; set; }
    [Required]
    public string MealName { get; set; }
}