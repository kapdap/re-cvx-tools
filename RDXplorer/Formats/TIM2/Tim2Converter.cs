using RDXplorer.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RDXplorer.Formats.TIM2
{
    public static class Tim2Converter
    {
        public static BitmapSource Decode(byte[] data, int index = 0, bool disableAlpha = false)
        {
            Tim2Document document = new(data);
            Tim2Picture picture = document.Pictures[index];
            return Decode(picture, disableAlpha);
        }

        public static BitmapSource Decode(Tim2Document document, int index = 0, bool disableAlpha = false) =>
            Decode(document.Pictures[index], disableAlpha);

        public static BitmapSource Decode(Tim2Picture picture, bool disableAlpha = false) =>
            BitmapSource.Create(
                picture.Width,
                picture.Height,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                ConvertToBgra32(picture, disableAlpha),
                picture.Width * 4);

        private static byte[] ConvertToBgra32(Tim2Picture picture, bool disableAlpha)
        {
            byte[] output = new byte[picture.Width * picture.Height * 4];
            byte[] clut = null;

            if (picture.ClutData != null)
                clut = ImageConversion.FixPalettePS2(picture.ClutData, (byte)picture.ClutColorType, (byte)picture.ImageColorType);

            switch (picture.ImageColorType)
            {
                case Tim2ColorType.RGBA32_A8B8G8R8:
                    ImageConversion.Rgba32ToBgra32(picture.ImageData, output, picture.Width * picture.Height);
                    break;

                case Tim2ColorType.RGB32_X8B8G8R8:
                    ImageConversion.Rgb32ToBgra32(picture.ImageData, output, picture.Width * picture.Height);
                    break;

                case Tim2ColorType.RGBA16_A1B5G5R5:
                    ImageConversion.Rgba16ToBgra32(picture.ImageData, output, picture.Width * picture.Height);
                    break;

                case Tim2ColorType.Indexed8Bit:
                    ImageConversion.Indexed8ToBgra32(picture.ImageData, clut, output, picture.Width * picture.Height);
                    break;

                case Tim2ColorType.Indexed4Bit:
                    ImageConversion.Indexed4ToBgra32(picture.ImageData, clut, output, picture.Width * picture.Height);
                    break;

                default:
                    if (picture.ImageData != null)
                        Array.Copy(picture.ImageData, output, Math.Min(picture.ImageData.Length, output.Length));
                    break;
            }

            if (disableAlpha)
                for (int i = 0; i < output.Length; i += 4)
                    output[i + 3] = 255;

            return output;
        }
    }
}