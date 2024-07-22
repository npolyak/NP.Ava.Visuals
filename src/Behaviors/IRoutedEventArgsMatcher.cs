using Avalonia.Interactivity;

namespace NP.Ava.Visuals.Behaviors
{
    public interface IRoutedEventArgsMatcher
    {
        bool Matches(RoutedEventArgs args);
    }
}
