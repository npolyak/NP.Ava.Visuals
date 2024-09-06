using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;

namespace NP.Ava.Visuals.Converters
{
    public class BitmapConverter : IValueConverter
    {
        public static BitmapConverter Instance { get; } = new BitmapConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string s = value?.ToString();
            var uri = s.StartsWith("/")
                ? new Uri(s, UriKind.Relative)
                : new Uri(s, UriKind.RelativeOrAbsolute);

            if (uri.IsAbsoluteUri && uri.IsFile)
                return new Bitmap(uri.LocalPath);

            Uri? baseUri = parameter as Uri;

            if (AssetLoader.Exists(uri, baseUri))
            {
                return new Bitmap(AssetLoader.Open(uri, baseUri));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
