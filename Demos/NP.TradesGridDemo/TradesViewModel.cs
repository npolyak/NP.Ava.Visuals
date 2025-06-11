using DynamicData.Binding;
using DynamicData;
using NP.Mockups.Finance;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using Avalonia.ReactiveUI;

namespace NP.TradesGridDemo;

public class TradesViewModel : IDisposable
{

    ObservableCollection<ITrade> _resultTrades = 
        new ObservableCollection<ITrade>();

    public ObservableCollection<ITrade> ResultTrades => 
        _resultTrades;

    IDisposable? _subscription = null;

    public TradesViewModel()
    {
        var metaTrade1 = Symbol.META.CreateTrade(2000);

        var oracleTrade1 = Symbol.ORCL.CreateTrade(1000);

        var metaTrade2 = Symbol.META.CreateTrade(1900); ;

        var oracleTrade2 = Symbol.ORCL.CreateTrade(900);

        // create the source collection of trades as
        // observable collection
        ObservableCollection<Trade> sourceTradeCollection =
            new ObservableCollection<Trade>
            {
                    metaTrade1, oracleTrade1, metaTrade2, oracleTrade2
            };


        // create stream of IChange<int> parameters
        // from the source collection
        IObservable<IChangeSet<Trade, int>> changeSetStream =
            sourceTradeCollection.ToObservableChangeSet(t => t.TradeId);

        // do the grouping
        IObservable<IGroupChangeSet<Trade, int, Symbol>>
            groupedObservable =
                changeSetStream.AutoRefresh().Group(t => t.TheSymbol);

        // transform the grouped entries into 
        // SymbolTradeGroup objects
        var transformedGroups =
            groupedObservable.Transform(g => new SymbolTradeGroup(g));

        var flattenedGroups =
            transformedGroups.MergeManyChangeSets
            (
                g => g.AggregationAndChildTrades.Connect()

            );
        //transformedGroups.TransformMany
        //(
        //    g => g.AggregationAndChildTrades,
        //    t => t.TradeId
        //);

        //output groups
        //ReadOnlyObservableCollection<ITrade>? symbolTradeGroups;

        // create and populate an observable collection
        // symbolTradeGroups that contains those SymbolTradeGroup
        // objects
        _subscription =
            flattenedGroups
                //.Bind(out symbolTradeGroups)
                .ObserveOn(AvaloniaScheduler.Instance)
                .SortAndBind
                (
                    _resultTrades,
                    SortExpressionComparer<ITrade>
                            .Ascending(t => t.TheSymbol.ToString())
                )
                
                .DisposeMany() // make sure that if an item is removed
                               // from the collection, it is disposed
                .Subscribe();  // start the subscription


        sourceTradeCollection.Add(Symbol.META.CreateTrade(1000));
        sourceTradeCollection.Add(Symbol.META.CreateTrade(2000));

        sourceTradeCollection.Add(Symbol.MSFT.CreateTrade(1000));

        metaTrade1 = Symbol.META.CreateTrade(20, metaTrade1.TradeId);

        sourceTradeCollection.Add(metaTrade1);
    }

    public void Dispose()
    {
        _subscription?.Dispose();
        _subscription = null;
    }
}
