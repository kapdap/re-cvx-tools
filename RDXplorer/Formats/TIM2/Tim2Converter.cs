using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using RDXplorer.Helpers;

namespace RDXplorer.Formats.TIM2
{
    public static class Tim2Converter
    {
        public static BitmapSource Decode(byte[] data, int index = 0)
        {
            Tim2Document document = new(data);
            Tim2Picture picture = document.Pictures[index];

            return BitmapSource.Create(
                picture.Width,
                picture.Height,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                ConvertToBgra32(picture.ImageData, picture.ClutData, picture.ImageColorType, picture.Width, picture.Height),
                picture.Width * 4);
        }

        private static byte[] ConvertToBgra32(byte[] input, byte[] clut, Tim2ColorType colors, int width, int height)
        {
            byte[] output = new byte[width * height * 4];

            switch (colors)
            {
                case Tim2ColorType.RGBA32_A8B8G8R8:
                    ImageConversion.Rgba32ToBgra32(input, output, width * height);
                    break;

                case Tim2ColorType.RGB32_X8B8G8R8:
                    ImageConversion.Rgb32ToBgra32(input, output, width * height);
                    break;

                case Tim2ColorType.RGBA16_A1B5G5R5:
                    ImageConversion.Rgba16ToBgra32(input, output, width * height);
                    break;

                case Tim2ColorType.Indexed8Bit:
                    ImageConversion.Indexed8ToBgra32(input, clut, output, width * height);
                    break;

                case Tim2ColorType.Indexed4Bit:
                    ImageConversion.Indexed4ToBgra32(input, clut, output, width * height);
                    break;

                default:
                    if (input != null)
                        Array.Copy(input, output, Math.Min(input.Length, output.Length));
                    break;
            }

            return output;
        }
    }
}