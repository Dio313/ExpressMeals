namespace ExpressMeals.Domains.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int MealId { get; set; }
        public Meal Meal { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string MealName { get; set; } 


    }
}
