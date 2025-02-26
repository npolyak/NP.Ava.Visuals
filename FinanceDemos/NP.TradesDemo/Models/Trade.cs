using NP.Utilities;
using System;
using System.ComponentModel;
using System.Threading;

using static NP.TradesDemo.Models.BuyOrSell;
using static NP.TradesDemo.Models.NasdaqSymbol;
using static NP.TradesDemo.Models.Customer;
using System.Text.Json.Serialization;
using DynamicData.Binding;


namespace NP.TradesDemo.Models;


public class Trade : VMBase
{
    static int _tradeIdGenerator = 0;

    public int Id { get; set; }

    public Customer TradeCustomer { get; set; }

    public BuyOrSell? TheBuyOrSell { get; set; }

    public NasdaqSymbol InstrumentNASDAQSymbol { get; set; } = NasdaqSymbol.None;

    public string? InstrumentCompanyName
        => InstrumentNASDAQSymbol.GetCompanyName();

    public Trader TheTrader { get; set; }

    #region TheTradeStatus Property
    private TradeStatus _tradeStatus = TradeStatus.Live;
    public TradeStatus TheTradeStatus
    {
        get
        {
            return this._tradeStatus;
        }
        set
        {
            if (this._tradeStatus == value)
            {
                return;
            }

            this._tradeStatus = value;
            this.OnPropertyChanged(nameof(TheTradeStatus));
        }
    }
    #endregion TheTradeStatus Property


    #region NumberShares Property
    private decimal _numberShares = 0;
    public decimal NumberShares
    {
        get
        {
            return this._numberShares;
        }
        set
        {
            if (this._numberShares == value)
            {
                return;
            }

            this._numberShares = value;
            this.OnPropertyChanged(nameof(NumberShares));
        }
    }
    #endregion NumberShares Property


    #region TradePricePerShareDollars Property
    private decimal _tradePricePerShareDollars = 0m;
    public decimal TradePricePerShareDollars
    {
        get
        {
            return this._tradePricePerShareDollars;
        }
        set
        {
            if (this._tradePricePerShareDollars == value)
            {
                return;
            }

            this._tradePricePerShareDollars = value;
            this.OnPropertyChanged(nameof(TradePricePerShareDollars));
        }
    }
    #endregion TradePricePerShareDollars Property

    public decimal TotalTradePrice => TradePricePerShareDollars * NumberShares;


    public static implicit operator 
        Trade
        (
            (
                int id,
                Customer tradeCustomer,
                BuyOrSell buyOrSell,
                NasdaqSymbol instrumentSymbol,
                Trader trader,
                TradeStatus tradeStatus,
                decimal numberOfShares,
                decimal tradePricePerShareDollars
            ) tuple
        ) =>
        new Trade
        (
            tuple.id, 
            tuple.tradeCustomer, 
            tuple.buyOrSell, 
            tuple.instrumentSymbol, 
            tuple.trader, 
            tuple.tradeStatus,
            tuple.numberOfShares,
            tuple.tradePricePerShareDollars
         );
    public Trade()
    {
        
    }

    public Trade
    (
        int id, 
        Customer tradeCustomer, 
        BuyOrSell buyOrSell, 
        NasdaqSymbol instrumentSymbol, 
        Trader trader,
        TradeStatus tradeStatus,
        decimal numberOfShares,
        decimal tradePricePerShareDollars
    )
    {
        this.Id = id;
        this.TradeCustomer = tradeCustomer;
        this.TheBuyOrSell = buyOrSell;
        this.InstrumentNASDAQSymbol = instrumentSymbol;
        this.TheTrader = trader;
        this.TheTradeStatus = tradeStatus;
        this.NumberShares = numberOfShares;
        this.TradePricePerShareDollars = tradePricePerShareDollars;
    }

    //public Trade(string customer, BuyOrSell theBuyOrSell)
    //{
    //    Id = Interlocked.Increment(ref _tradeIdGenerator);
    //    Customer = customer;
    //    TheBuyOrSell = theBuyOrSell;
    //}
}

public static class TradesHelper
{
    public static Trade[] TestTrades { get; } =
    [
        (
            1,
            AmericanExpress, 
            Buy, 
            MSFT, 
            Trader.Jack, 
            TradeStatus.Live, 
            1000m, 
            111m
        ),
        (
            2,
            BankOfAmerica,
            Buy,
            TSLA,
            Trader.Ami,
            TradeStatus.Live,
            800m,
            211m
        )
    ];
}