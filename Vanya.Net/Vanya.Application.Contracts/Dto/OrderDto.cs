using Vanya.Core.Enums;

namespace Vanya.Application.Contracts.Dto;

public class OrderDto
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public BidAsk BidAsk { get; set; }
}
