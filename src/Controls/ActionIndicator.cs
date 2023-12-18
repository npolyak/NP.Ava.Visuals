using Avalonia;
using Avalonia.Controls.Primitives;

namespace NP.Avalonia.Visuals.Controls
{
    public class ActionIndicator : TemplatedControl
    {
        #region IsOn Styled Avalonia Property
        public bool IsOn
        {
            get { return GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        public static readonly StyledProperty<bool> IsOnProperty =
            AvaloniaProperty.Register<ActionIndicator, bool>
            (
                nameof(IsOn)
            );
        #endregion IsOn Styled Avalonia Property
    }
}
