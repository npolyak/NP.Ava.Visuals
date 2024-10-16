using Avalonia.Platform;
using NP.Ava.Visuals.Converters;

namespace NP.Ava.Visuals.Converters;

public class BoolToChromeHintsConverter : GenericBoolConverter<ExtendClientAreaChromeHints>
{
    public static BoolToChromeHintsConverter Instance { get; } =
        new BoolToChromeHintsConverter
        {
            TrueValue = ExtendClientAreaChromeHints.NoChrome,
            FalseValue = ExtendClientAreaChromeHints.SystemChrome
        };
}
