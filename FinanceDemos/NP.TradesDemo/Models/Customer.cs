using System;
using System.Linq;
using NP.Utilities;

namespace NP.TradesDemo.Models;

public enum Customer
{
    None, 
    AmericanExpress,
    Fidelity,
    BankOfAmerica,
    FranklinTempleton,
    MorganWealth,
    CharlesSchwab,
    GoldmanSachs
}

public static class CustomerHelper
{
    public static Customer[] Customers { get; } =
        Enum.GetValues<Customer>()
            .Except([Customer.None])
            .ToArray();

    public static string GetName(this Customer customer)
    {
        return customer.ToString().PhraseFromName();
    }
}