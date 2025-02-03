using System;
using Trader.Domain.Model;


namespace NP.Demos.LiveTradingDemo.Domain.Services;

public interface IMarketDataService
{
    IObservable<MarketData> Watch(string currencyPair);
}