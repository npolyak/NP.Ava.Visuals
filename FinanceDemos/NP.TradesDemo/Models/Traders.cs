using System;
using System.Linq;

namespace NP.TradesDemo.Models;

public enum Trader
{
    None,
    Joe,
    Jack,
    Jim,
    Jordan,
    Ami, 
    Nancy
}

public static class TradersHelper
{
    public static Trader[] Traders { get; } = 
        Enum.GetValues<Trader>()
            .Except([Trader.None])
            .ToArray();
}