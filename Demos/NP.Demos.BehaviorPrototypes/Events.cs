using Avalonia.Interactivity;

namespace NP.Demos.BehaviorPrototypes
{
    public class Events
    {
        /// <summary>
        /// Defines the <see cref="PointerPressed"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> MyEvent =
            RoutedEvent.Register<Events, RoutedEventArgs>
            (
                "My",
                RoutingStrategies.Bubble);
    }
}
