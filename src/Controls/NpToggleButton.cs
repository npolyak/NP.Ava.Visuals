using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;

namespace NP.Avalonia.Visuals.Controls
{
    [PseudoClasses(":checked", ":unchecked")]
    public class NpToggleButton : TemplatedControl
    {
        #region IsChecked Styled Avalonia Property
        public static readonly DirectProperty<NpToggleButton, bool> IsCheckedProperty =
            AvaloniaProperty.RegisterDirect<NpToggleButton, bool>(
                nameof(IsChecked),
                o => o.IsChecked,
                (o, v) => o.IsChecked = v,
                unsetValue: false,
                defaultBindingMode: BindingMode.TwoWay);
        #endregion IsChecked Styled Avalonia Property

        private bool _isChecked = false;
        /// <summary>
        /// Gets or sets whether the <see cref="ToggleButton"/> is checked.
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                SetAndRaise(IsCheckedProperty, ref _isChecked, value);
                UpdatePseudoClasses(IsChecked);
            }
        }

        private void UpdatePseudoClasses(bool? isChecked)
        {
            PseudoClasses.Set(":checked", isChecked == true);
            PseudoClasses.Set(":unchecked", isChecked == false);
        }

        private void ToggleState()
        {
            IsChecked = !IsChecked;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key== Key.Enter)
            {
                ToggleState();
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            ToggleState();
        }
    }
}
