using Avalonia.Controls;

namespace NP.Ava.Visuals
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
