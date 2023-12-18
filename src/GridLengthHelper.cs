using Avalonia.Controls;
using System.Globalization;

namespace NP.Avalonia.Visuals
{
    public static class GridLengthHelper
    {
        public static string ToCultureInvariantStr(this GridLength gridLength)
        {
            if (gridLength.IsAuto)
            {
                return "Auto";
            }

            string s = gridLength.Value.ToString(CultureInfo.InvariantCulture);
            return gridLength.IsStar ? s + "*" : s;
        }
    }
}
