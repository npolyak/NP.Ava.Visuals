using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using NP.Demos.LiveTradingDemo.Domain.Infrastructure;
using NP.Demos.LiveTradingDemo.Domain.Model;
using NP.Demos.LiveTradingDemo.Domain.Services;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace NP.Demos.LiveTradingDemo.ViewModels;

public class LiveTradesViewModel : AbstractNotifyPropertyChanged, IDisposable
{
    private readonly ReadOnlyObservableCollection<TradeProxy> _data;
    public ReadOnlyObservableCollection<TradeProxy> Data => _data;

    public SearchHints SearchHints { get; }

    public IStaticData StaticData { get; } = new StaticData();

    public IMarketDataService MarketDataService { get; } 

    bool _paused;
    public bool Paused
    {
        get => _paused;
        set => SetAndRaise(ref _paused, value);
    }

    private readonly IDisposable _cleanUp;
    public LiveTradesViewModel ()
    {
        this.MarketDataService = new MarketDataService(StaticData);

        SchedulerProvider schedulerProvider = new SchedulerProvider(Dispatcher.UIThread);

        TradeGenerator tradeGenerator = new TradeGenerator(StaticData, MarketDataService);

        ITradeService tradeService = new TradeService(tradeGenerator, schedulerProvider);

        SearchHints = new SearchHints(tradeService, schedulerProvider);

        IObservable<Func<Trade, bool>> filter =
            SearchHints.WhenValueChanged(t => t.SearchText)
                       .Select(BuildFilter);

        IDisposable loader =
            tradeService
                .Live
                .Connect()

                .BatchIf(this.WhenValueChanged(x => x.Paused), null, null)
                .Filter(filter)
                .Transform(trade => new TradeProxy(trade))
                .Sort
                (
                    SortExpressionComparer<TradeProxy>.Descending(t => t.Timestamp),
                    SortOptimisations.ComparesImmutableValuesOnly,
                    25
                )
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out _data)
                .DisposeMany() // dispose removed TradeProxy objects
                .Subscribe();

        _cleanUp = new CompositeDisposable(loader, SearchHints);
    }

    private Func<Trade, bool> BuildFilter(string searchText)
    {
        if (string.IsNullOrEmpty(searchText)) 
            return trade => true;

        return t => t.CurrencyPair.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        || t.Customer.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}
