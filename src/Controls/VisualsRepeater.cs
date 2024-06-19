using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using System;

namespace NP.Ava.Visuals.Controls
{
    public class VisualsRepeater : ItemsControl
    {
        protected override Type StyleKeyOverride => typeof(ItemsControl);

        #region NumberOfTimesToRepeat Styled Avalonia Property
        public int NumberOfTimesToRepeat
        {
            get { return GetValue(NumberOfTimesToRepeatProperty); }
            set { SetValue(NumberOfTimesToRepeatProperty, value); }
        }

        public static readonly StyledProperty<int> NumberOfTimesToRepeatProperty =
            AvaloniaProperty.Register<VisualsRepeater, int>
            (
                nameof(NumberOfTimesToRepeat)
            );
        #endregion NumberOfTimesToRepeat Styled Avalonia Property

        IDisposable _subscription;
        public VisualsRepeater()
        {
            _subscription = this.GetObservable<int>(NumberOfTimesToRepeatProperty).Subscribe(OnNumberOfTimesToRepeatPropertyChanged);
        }

        private void OnNumberOfTimesToRepeatPropertyChanged(int numberOfTimesToRepeat)
        {
            this.ItemsSource = new bool[numberOfTimesToRepeat];
        }
    }
}
