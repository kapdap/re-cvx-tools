using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RDXplorer.Formats.PVR
{
    public static class PvrConverter
    {
        public static BitmapSource Decode(byte[] data, byte[] paletteRgba = null, bool disableAlpha = false)
        {
            PvrDocument document = new(data, paletteRgba);
            return Decode(document.Pictures[0], disableAlpha);
        }

        public static BitmapSource Decode(PvrDocument document, int index = 0, bool disableAlpha = false) =>
            Decode(document.Pictures[index], disableAlpha);

        public static BitmapSource Decode(PvrPicture picture, bool disableAlpha = false) =>
            BitmapSource.Create(
                picture.Width,
                picture.Height,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                ConvertToBgra32(picture, disableAlpha),
                picture.Width * 4);

        private static byte[] ConvertToBgra32(PvrPicture picture, bool disableAlpha)
        {
            byte[] rgba = picture.Pixels;
            byte[] output = new byte[rgba.Length];
            Buffer.BlockCopy(rgba, 0, output, 0, rgba.Length);

            // RGBA -> BGRA for WPF.
            for (int i = 0; i < output.Length; i += 4)
            {
                byte r = output[i];
                output[i] = output[i + 2];
                output[i + 2] = r;

                if (disableAlpha)
                    output[i + 3] = 255;
            }

            return output;
        }
    }
}
