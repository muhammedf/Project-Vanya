using WebApplication1.Core.Model;

namespace WebApplication1.Core.Engine
{
    public interface IMarket
    {
        Instrument Instrument{ get; }
        void AddOrder(Order order);
        void CancelOrder(Order order);
        void EditOrder(Order order);

        event EventHandler<Order> OrderAdded;
        event EventHandler<Order> OrderCancelled;
        event EventHandler<(Order old, Order @new)> OrderEdited;
        event EventHandler<Deal> DealCreated;
    }
}
