namespace ExpressMeals.Domains.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public List<Meal> Meals { get; set; }
}