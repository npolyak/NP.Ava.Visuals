using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace NP.Demos.LiveTradingDemo.Domain.Model;

public class TradeProxy : AbstractNotifyPropertyChanged, IDisposable, IEquatable<TradeProxy>
{
    private readonly IDisposable _cleanUp;
    private readonly long _id;
    private readonly Trade _trade;

    private bool _recent;
    public bool Recent
    {
        get => _recent;
        set => SetAndRaise(ref _recent, value);
    }


    private decimal _marketPrice;
    public decimal MarketPrice
    {
        get => _marketPrice;
        set => SetAndRaise(ref _marketPrice, value);
    }


    private decimal _pcFromMarketPrice;
    public decimal PercentFromMarket
    {
        get => _pcFromMarketPrice;
        set => SetAndRaise(ref _pcFromMarketPrice, value);
    }


    #region Delegating Members

    public long Id => _trade.Id;

    public string CurrencyPair => _trade.CurrencyPair;

    public string Customer => _trade.Customer;

    public decimal Amount => _trade.Amount;

    public TradeStatus Status => _trade.Status;

    public DateTime Timestamp => _trade.Timestamp;

    public decimal TradePrice => _trade.TradePrice;

    #endregion

    public TradeProxy(Trade trade)
    {
        _id = trade.Id;
        _trade = trade;

        var isRecent = DateTime.Now.Subtract(trade.Timestamp).TotalSeconds < 2;
        var recentIndicator = Disposable.Empty;

        if (isRecent)
        {
            Recent = true;
            recentIndicator =
                Observable
                    .Timer(TimeSpan.FromSeconds(2))
                    .Subscribe(_ => Recent = false);
        }

        //market price changed is an observable on the trade object
        var priceRefresher =
            trade.MarketPriceChanged
                .Subscribe(_ =>
                {
                    MarketPrice = trade.MarketPrice;
                    PercentFromMarket = trade.PercentFromMarket;
                });

        _cleanUp = Disposable.Create(() =>
        {
            recentIndicator.Dispose();
            priceRefresher.Dispose();
        });
    }


    public void Dispose()
    {
        _cleanUp.Dispose();
    }

    #region Equaility Members

    public bool Equals(TradeProxy other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _id == other._id;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TradeProxy) obj);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public static bool operator ==(TradeProxy left, TradeProxy right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TradeProxy left, TradeProxy right)
    {
        return !Equals(left, right);
    }

    #endregion
}