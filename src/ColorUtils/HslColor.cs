using NP.Utilities;

namespace NP.Ava.Visuals.ColorUtils
{
    public record HslColor(byte A, float H, float S, float L)
    {
        public override string ToString()
        {
            return $"{A}, {H}, {(S * 100d).ToFixed(1)}%, {(L * 100).ToFixed(1)}%";
        }
    }
}
