using DynamicData;
using DynamicData.Binding;
using NP.Demos.LiveTradingDemo.Domain.Infrastructure;
using NP.Demos.LiveTradingDemo.Domain.Model;
using NP.Demos.LiveTradingDemo.Domain.Services;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace NP.Demos.LiveTradingDemo.ViewModels;

public class SearchHints : 
    AbstractNotifyPropertyChanged, 
    IDisposable
{
    private readonly IDisposable _cleanUp;

    private readonly ReadOnlyObservableCollection<string> _hints;
    public ReadOnlyObservableCollection<string> Hints => _hints;

    private string? _searchText;

    public string? SearchText
    {
        get => _searchText;
        set => SetAndRaise(ref _searchText, value);
    }

    private static Func<string, bool> BuildFilter(string? searchText)
    {
        if (string.IsNullOrEmpty(searchText)) 
            return trade => true;

        return str => 
                str.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }


    public SearchHints
    (
        ITradeService tradeService, 
        ISchedulerProvider schedulerProvider)
    {
        //build a predicate when SearchText changes
        IObservable<Func<string, bool>> filter = 
            this.WhenValueChanged(x => x.SearchText)
                .Throttle(TimeSpan.FromMicroseconds(250))
                .Select(BuildFilter);

        // share the connection
        IConnectableObservable<IChangeSet<Trade, long>> shared = 
            tradeService.All.Connect().Publish();

        //distict observable of customers
        IObservable<IDistinctChangeSet<string>> distinctCustomers = 
            shared.DistinctValues(trade => trade.Customer);

        //distinct observables or currency pairs
        var distinctCurrencyPairs = shared.DistinctValues(trade => trade.CurrencyPair);

        // observe customers and currency pairs using OR operation and
        // bind to the observable collection
        IDisposable loader =
            distinctCustomers.Or(distinctCurrencyPairs)
                             .Filter(filter)
                             .Sort(SortExpressionComparer<string>.Ascending(s => s))
                             .ObserveOn(schedulerProvider.MainThread)
                             .Bind(out _hints)
                             .Subscribe();

        _cleanUp = new CompositeDisposable(loader, shared.Connect());
    }

    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}
