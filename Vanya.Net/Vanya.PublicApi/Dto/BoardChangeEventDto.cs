using WebApplication1.Core.Enum;

namespace Vanya.PublicApi.Dto
{
    public class BoardChangeEventDto
    {
        public enum EventTypeEnum
        {
            AddOrder, RemoveOrder, NewDeal
        }

        public EventTypeEnum EventType { get; set; }
        public OrderDto EventData { get; set; }
        public string OldTrackingHash { get; set; }
        public string NewTrackingHash { get; set; }
    }
}
