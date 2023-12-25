using Avalonia.Styling;
using NP.Utilities;
using System.ComponentModel;

namespace NP.Ava.Visuals.ThemingAndL10N
{
    public class ThemeVariantReference : VMBase
    {
        #region TheThemeVariant Property
        private ThemeVariant? _themeVariant;
        public ThemeVariant? TheThemeVariant
        {
            get
            {
                return this._themeVariant;
            }
            set
            {
                if (this._themeVariant.ObjEquals(value))
                {
                    return;
                }

                this._themeVariant = value;
                this.OnPropertyChanged(nameof(TheThemeVariant));
            }
        }
        #endregion TheThemeVariant Property
    }
}
