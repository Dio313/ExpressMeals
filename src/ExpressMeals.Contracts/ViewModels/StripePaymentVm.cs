namespace ExpressMeals.Contracts.ViewModels;

public class StripePaymentVm
{
    public StripePaymentVm(OrderVm order)
    {
        Order = order;
        SuccessUrl = "OrderConfirmation";
        CancelUrl = "Summary";
    }
    public OrderVm Order { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
}