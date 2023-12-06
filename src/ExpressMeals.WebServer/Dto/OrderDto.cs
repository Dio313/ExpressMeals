using ExpressMeals.Domains.Entities;

namespace ExpressMeals.WebServer.Dto;

public class OrderDto
{
    public Order OrderHeader { get; set; }
    public IEnumerable<OrderDetail> OrderDetails { get; set; }
}