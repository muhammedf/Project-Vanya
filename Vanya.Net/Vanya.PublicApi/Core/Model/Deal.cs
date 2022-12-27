namespace WebApplication1.Core.Model
{
    public class Deal
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Order BuyOrder { get; set; }
        public Order SellOrder { get; set; }
    }
}
