using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace NP.Ava.Visuals
{
    public static class ImageHelper
    {
        public static IImage ControlToImage(this Control control)
        {
            double width = control.Bounds.Width;
            double height = control.Bounds.Height;

            PixelSize pixSize =
                new PixelSize((int)width, (int)height);

            var dpi = new Avalonia.Vector(96d, 96d);

            using RenderTargetBitmap renderTargetBitmap =
                new RenderTargetBitmap(pixSize, dpi);
            var size = new Size(width, height);

            renderTargetBitmap.Render(control);

            WriteableBitmap writeableBitmap = new WriteableBitmap(pixSize, dpi);

            var buff = writeableBitmap.Lock();

            renderTargetBitmap.CopyPixels
            (
                new PixelRect(new PixelPoint(0, 0), pixSize),
                buff.Address,
                sizeof(int) * buff.Size.Width * buff.Size.Height,
                buff.RowBytes);

            Bitmap b =
                new Bitmap
                (
                    renderTargetBitmap.Format.Value,
                    AlphaFormat.Premul,
                    buff.Address,
                    pixSize,
                    dpi,
                    buff.RowBytes);

            return b;
        }
    }
}
