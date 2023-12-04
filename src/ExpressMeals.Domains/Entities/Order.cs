namespace ExpressMeals.Domains.Entities
{
    public class Order : BaseEntity
    {

        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public string Status { get; set; }
        public string SessionId { get; set; }
        public string PaymentIntentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string Tracking { get; set; }
        public string Carrier { get; set; }
    }
}
