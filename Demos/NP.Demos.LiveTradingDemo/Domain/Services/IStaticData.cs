using Trader.Domain.Model;

namespace NP.Demos.LiveTradingDemo.Domain.Services;

public interface IStaticData
{
    string[] Customers { get; }
    CurrencyPair[] CurrencyPairs { get; }
}