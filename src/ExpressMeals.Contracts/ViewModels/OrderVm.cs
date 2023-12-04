namespace ExpressMeals.Contracts.ViewModels;

public class OrderVm
{
    public OrderCreateVm OrderHeader { get; set; }
    public List<OrderDetailVm> OrderDetails { get; set; }
}