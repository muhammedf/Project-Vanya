using WebApplication1.Core.Enum;

namespace WebApplication1.Core.Model
{
    public class Order
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public BidAsk BidAsk { get; set; }
        public Instrument Instrument { get; set; }
        public Trader Owner { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
