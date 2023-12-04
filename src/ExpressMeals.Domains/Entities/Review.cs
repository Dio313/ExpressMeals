namespace ExpressMeals.Domains.Entities;

public class Review : BaseEntity
{
    public int Rate { get; set; }
    public DateTime RatingDate { get; set; }
    public int MealId { get; set; }
    public Meal Meal { get; set; }
    public string UserId { get; set; }

}