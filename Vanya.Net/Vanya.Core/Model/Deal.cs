namespace Vanya.Core.Model;

public class Deal
{
    public long Id { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Order BidOrder { get; set; }
    public Order AskOrder { get; set; }
}
