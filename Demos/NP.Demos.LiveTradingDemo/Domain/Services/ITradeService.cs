using DynamicData;
using NP.Demos.LiveTradingDemo.Domain.Model;

namespace NP.Demos.LiveTradingDemo.Domain.Services;

public interface ITradeService
{
    IObservableCache<Trade, long> All { get; }
    IObservableCache<Trade, long> Live { get; }
}
