using WebApplication1.Core.Enum;

namespace Vanya.PublicApi.Dto
{
    public class OrderDto
    {
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public BidAsk BidAsk { get; set; }
    }
}
