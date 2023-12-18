using Avalonia.Controls;

namespace NP.Avalonia.Visuals
{
    internal class ControlContainer
    {
        public Control Control { get; }

        internal ControlContainer(Control control)
        {
            Control = control;
        }
    }
}
