using Avalonia.Input;
using Avalonia;
using NP.Utilities;

namespace NP.Ava.Visuals.Behaviors
{
    public static class KeyboardService
    {
        private static KeyboardDevice? _instance;
        public static KeyboardDevice? Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (KeyboardDevice)AvaloniaLocator.Current.GetService<IKeyboardDevice>();
                }

                return _instance;
            }
        }

        public static bool IsShiftPressed
        {
            get
            {
                RawInputModifiers modifiers =
                    (RawInputModifiers) Instance.GetPropValue("Modifiers", true);

                return (modifiers & RawInputModifiers.Shift) == RawInputModifiers.Shift;
            }
        }

        public static bool IsControlPressed
        {
            get
            {
                RawInputModifiers modifiers =
                    (RawInputModifiers)Instance.GetPropValue("Modifiers", true);

                return (modifiers & RawInputModifiers.Control) == RawInputModifiers.Control;
            }
        }
    }
}
