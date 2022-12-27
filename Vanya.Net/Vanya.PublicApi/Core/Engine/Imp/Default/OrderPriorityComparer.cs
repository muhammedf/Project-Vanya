using WebApplication1.Core.Model;

namespace WebApplication1.Core.Engine.Imp.Default
{
    public class OrderPriorityComparer : IComparer<Order>
    {
        public static OrderPriorityComparer Instance { get; } = new OrderPriorityComparer();

        public int Compare(Order? x, Order? y)
        {
            if (x.CreateTime > y.CreateTime) return 1;
            else if (x.CreateTime < y.CreateTime) return -1;
            return 0;
        }
    }
}
