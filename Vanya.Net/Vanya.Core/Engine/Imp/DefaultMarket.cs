using System.Collections.Concurrent;
using Vanya.Core.Enums;
using Vanya.Core.Model;

namespace Vanya.Core.Engine.Imp;

public class DefaultMarket : IMarket
{
    static readonly IComparer<decimal> descendingDecimalComparer = Comparer<decimal>.Create((x, y) => -Comparer<decimal>.Default.Compare(x, y));
    static readonly IComparer<Order> byDateComparer = Comparer<Order>.Create((x, y) => Comparer<DateTime>.Default.Compare(x.CreateTime, y.CreateTime));

    private readonly SortedList<decimal, SortedSet<Order>> _asks;
    private readonly SortedList<decimal, SortedSet<Order>> _bids;
    private readonly List<Deal> _deals;
    private readonly ConcurrentDictionary<long, Order> _orders;
    private readonly Instrument _instrument;

    public event EventHandler<Order>? OrderAdded;
    public event EventHandler<Order>? OrderCancelled;
    public event EventHandler<(Order old, Order @new)>? OrderEdited;
    public event EventHandler<Deal>? DealCreated;

    public Instrument Instrument => _instrument;

    private int _orderId;
    private long _dealId;

    public DefaultMarket(Instrument instrument)
    {
        _asks = new SortedList<decimal, SortedSet<Order>>();
        _bids = new SortedList<decimal, SortedSet<Order>>(descendingDecimalComparer);
        _deals = new List<Deal>();
        _orders = new ConcurrentDictionary<long, Order>();
        _instrument = instrument;
    }

    public void AddOrder(Order order)
    {
        order.Id = Interlocked.Increment(ref _orderId);
        order.CreateTime = DateTime.Now;

        AddOrderInternal(order);

        OrderAdded?.Invoke(this, order);

        Execute();
    }

    private void AddOrderInternal(Order order)
    {
        if (!_orders.TryAdd(order.Id, order))
        {
            throw new ArgumentException("Order already exists.");
        }

        var list = order.BidAsk == BidAsk.Ask ? _asks : _bids;
        if (list.TryGetValue(order.Price, out SortedSet<Order>? value))
        {
            value.Add(order);
        }
        else
        {
            list.Add(order.Price, new SortedSet<Order>(byDateComparer) { order });
        }
    }

    private void Execute()
    {
        bool dealt;
        do
        {
            dealt = false;
            if (!_bids.Any() || !_asks.Any())
                continue;
            var bid = _bids.First().Value.First();
            var ask = _asks.First().Value.First();
            if (bid.Price >= ask.Price)
            {
                CreateDeal(bid, ask);
                dealt = true;
            }
        }
        while (dealt);
    }

    private void CreateDeal(Order bid, Order ask)
    {
        var deal = new Deal()
        {
            Id = Interlocked.Increment(ref _dealId),
            BidOrder = bid,
            AskOrder = ask,
            Price = ask.CreateTime <= bid.CreateTime ? ask.Price : bid.Price,
            Quantity = ask.Quantity <= bid.Quantity ? ask.Quantity : bid.Quantity
        };

        _deals.Add(deal);

        RemoveOrder(bid, bid.Price);
        RemoveOrder(ask, ask.Price);

        if (bid.Quantity > deal.Quantity)
        {
            bid.Quantity -= deal.Quantity;
            AddOrderInternal(bid);
        }
        if (ask.Quantity > deal.Quantity)
        {
            ask.Quantity -= deal.Quantity;
            AddOrderInternal(ask);
        }

        DealCreated?.Invoke(this, deal);
    }

    private void RemoveOrder(Order order, decimal price)
    {
        if (!_orders.TryRemove(order.Id, out _))
        {
            throw new ArgumentException("Order not found.");
        }

        var list = order.BidAsk == BidAsk.Ask ? _asks : _bids;

        if (list.TryGetValue(price, out SortedSet<Order>? value))
        {
            if (value.Contains(order))
            {
                value.Remove(order);
            }
            else
            {
                throw new ArgumentException("Order not found.");
            }
            if (!value.Any())
            {
                list.Remove(price);
            }
        }
        else
        {
            throw new ArgumentException("Order not found.");
        }
    }

    public void CancelOrder(Order order)
    {
        RemoveOrder(order, order.Price);
        OrderCancelled?.Invoke(this, order);
    }

    public void EditOrder(Order order)
    {
        if (!_orders.TryGetValue(order.Id, out Order? oldOrder))
        {
            throw new ArgumentException("Order not found.");
        }
        RemoveOrder(order, oldOrder.Price);
        AddOrderInternal(order);

        OrderEdited?.Invoke(this, (oldOrder, order));

        Execute();
    }
}
