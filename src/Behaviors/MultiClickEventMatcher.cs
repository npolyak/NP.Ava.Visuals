using Avalonia.Input;
using Avalonia.Interactivity;

namespace NP.Ava.Visuals.Behaviors
{
    public class MultiClickEventMatcher : IRoutedEventArgsMatcher
    {
        public int NumberClicked { get; set; } = 2;

        public bool Matches(RoutedEventArgs args)
        {
            if (args.RoutedEvent != InputElement.PointerPressedEvent)
                return false;

            PointerPressedEventArgs pointerPressedEventArgs = (PointerPressedEventArgs)args;

            return pointerPressedEventArgs.ClickCount == NumberClicked;
        }
    }
}
