using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Vanya.Application.Contracts.Dto;
using Vanya.Application.Contracts.ServiceContracts;
using Vanya.Core.Engine;
using Vanya.Core.Engine.Imp;
using Vanya.Core.Model;

namespace Vanya.Application.Services;

internal class DefaultTradingService : ITradingService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IInstrumentService _instrumentService;
    readonly List<IMarket> _markets;
    readonly ConcurrentDictionary<object, string> hashdict;

    public DefaultTradingService(IServiceProvider serviceProvider, IInstrumentService instrumentService)
    {
        hashdict = new ConcurrentDictionary<object, string>();

        var insts = instrumentService.GetActiveInstruments();

        _markets = insts.Select(x => new DefaultMarket(x)).Cast<IMarket>().ToList();

        foreach (var market in _markets)
        {
            market.OrderAdded += Market_OrderAdded;
            market.OrderEdited += Market_OrderEdited;
            market.OrderCancelled += Market_OrderCancelled;
            market.DealCreated += Market_DealCreated;
        }

        _serviceProvider = serviceProvider;
        _instrumentService = instrumentService;
    }

    #region BoardEvents

    private async Task SendBoardChange(object market, string instrumentName, BoardChangeEventDto data)
    {
        using var scope = _serviceProvider.CreateScope();
        var broadcast = scope.ServiceProvider.GetService<IBroadcastService>();

        string? oldHash = null;
        string newHash;
        lock (market)
        {
            hashdict.TryGetValue(market, out oldHash);
            hashdict[market] = newHash = Guid.NewGuid().ToString();
            data.OldTrackingHash = oldHash;
            data.NewTrackingHash = newHash;
        }

        await broadcast.SendToGroup(Constants.BOARD_CHANNEL, instrumentName, data);
    }

    private async void Market_OrderEdited(object? sender, (Order old, Order @new) e)
    {
        var boardchange = new BoardChangeEventDto()
        {
            EventType = BoardChangeEventDto.EventTypeEnum.RemoveOrder,
            EventData = new OrderDto()
            {
                Price = e.old.Price,
                Quantity = e.old.Quantity,
                BidAsk = e.old.BidAsk,
            }
        };

        await SendBoardChange(sender, e.old.Instrument.Name, boardchange);

        var boardchange2 = new BoardChangeEventDto()
        {
            EventType = BoardChangeEventDto.EventTypeEnum.AddOrder,
            EventData = new OrderDto()
            {
                Price = e.@new.Price,
                Quantity = e.@new.Quantity,
                BidAsk = e.@new.BidAsk,
            }
        };

        await SendBoardChange(sender, e.@new.Instrument.Name, boardchange2);
    }

    private async void Market_OrderAdded(object? sender, Order e)
    {
        var boardchange = new BoardChangeEventDto()
        {
            EventType = BoardChangeEventDto.EventTypeEnum.AddOrder,
            EventData = new OrderDto()
            {
                Price = e.Price,
                Quantity = e.Quantity,
                BidAsk = e.BidAsk,
            }
        };

        await SendBoardChange(sender, e.Instrument.Name, boardchange);
    }

    private async void Market_DealCreated(object? sender, Deal e)
    {
        var boardchange = new BoardChangeEventDto()
        {
            EventType = BoardChangeEventDto.EventTypeEnum.NewDeal,
            EventData = new OrderDto()
            {
                Price = e.Price,
                Quantity = e.Quantity,
            }
        };

        await SendBoardChange(sender, e.AskOrder.Instrument.Name, boardchange);
    }

    private async void Market_OrderCancelled(object? sender, Order e)
    {
        var boardchange = new BoardChangeEventDto()
        {
            EventType = BoardChangeEventDto.EventTypeEnum.RemoveOrder,
            EventData = new OrderDto()
            {
                Price = e.Price,
                Quantity = e.Quantity,
                BidAsk = e.BidAsk,
            }
        };

        await SendBoardChange(sender, e.Instrument.Name, boardchange);
    }

    #endregion

}
