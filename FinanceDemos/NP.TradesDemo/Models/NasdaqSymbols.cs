namespace NP.TradesDemo.Models;

using System.Linq;
using static NP.TradesDemo.Models.NasdaqSymbol;

public enum NasdaqSymbol
{
    None = 0,
    CVSI,
    TSLA,
    AAPL,
    AMZN,
    INTC,
    GOOG,
    IBMW,
    MSFT,
    ORCL,
    NVDA,
    META,
    MERC
}

public class CompanyData
{
    public NasdaqSymbol Symbol { get; init; }
    public string? CompanyName { get; init; }

    public static implicit operator CompanyData((NasdaqSymbol symbol, string companyName) t) =>
        new CompanyData { Symbol = t.symbol, CompanyName = t.companyName };
}

public static class Companies
{
    public static CompanyData[] TheCompanies { get; } =
        [
            (CVSI, "SV Sciences Inc"),
            (TSLA, "Tesla Inc"),
            (AAPL, "Apple Inc"),
            (AMZN, "Amazon.com Inc"),       
            (INTC, "Intel Inc"),
            (GOOG, "Google Inc"),
            (IBMW, "IBM Inc"),
            (MSFT, "Microsoft Corp"),
            (ORCL, "Oracle Corp"),
            (NVDA, "NVIDIA Corp"),
            (META, "Meta Platforms Inc"),
            (MERC, "Mercer International Inc")
        ];

    public static CompanyData? GetCompData(this NasdaqSymbol nasdaqSymbol)
    {
        return TheCompanies.FirstOrDefault(compData => compData.Symbol == nasdaqSymbol);
    }

    public static string? GetCompanyName(this NasdaqSymbol symbol)
    {
        return symbol.GetCompData()?.CompanyName;
    }
}