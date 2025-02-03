using System;
using System.Reactive.Concurrency;
using Avalonia.Threading;

namespace NP.Demos.LiveTradingDemo.Domain.Infrastructure
{
    public class SchedulerProvider : ISchedulerProvider
    {
        public SchedulerProvider(Dispatcher dispatcher)
        {
            MainThread = new DispatcherScheduler(dispatcher);
        }

        public IScheduler MainThread { get; }
        public IScheduler Background => TaskPoolScheduler.Default;
    }
}
