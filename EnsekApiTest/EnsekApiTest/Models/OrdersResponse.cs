namespace EnsekApiTest.Models
{
    public class OrdersResponse
    {
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public string fuel { get; set; }
        public string id { get; set; }
        public int quantity { get; set; }
        public string time { get; set; }
    }

    
}


