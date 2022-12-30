using Vanya.Core.Enums;

namespace Vanya.Application.Contracts.Dto;

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
