using System.Reactive.Concurrency;

namespace NP.Demos.LiveTradingDemo.Domain.Infrastructure
{
    public interface ISchedulerProvider
    {
        IScheduler MainThread { get; }
        IScheduler Background { get; }
    }
}
