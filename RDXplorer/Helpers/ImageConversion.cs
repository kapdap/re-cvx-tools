namespace RDXplorer.Helpers
{
    public static class ImageConversion
    {
        public static void Rgba32ToBgra32(byte[] input, byte[] output, int pixels)
        {
            for (int i = 0; i < pixels; i++)
            {
                int src = i * 4;
                int dst = i * 4;
                output[dst + 0] = input[src + 2]; // B
                output[dst + 1] = input[src + 1]; // G
                output[dst + 2] = input[src + 0]; // R
                output[dst + 3] = input[src + 3]; // A
            }
        }

        public static void Rgb32ToBgra32(byte[] input, byte[] output, int pixels)
        {
            for (int i = 0; i < pixels; i++)
            {
                int src = i * 4;
                int dst = i * 4;
                output[dst + 0] = input[src + 2]; // B
                output[dst + 1] = input[src + 1]; // G
                output[dst + 2] = input[src + 0]; // R
                output[dst + 3] = 255;            // A (opaque)
            }
        }

        public static void Rgba16ToBgra32(byte[] input, byte[] output, int pixels)
        {
            for (int i = 0; i < pixels; i++)
            {
                int src = i * 2;
                int dst = i * 4;
                ushort pixel = (ushort)(input[src] | (input[src + 1] << 8));

                // A1B5G5R5 to BGRA32
                int r = (pixel & 0x1F) * 255 / 31;
                int g = ((pixel >> 5) & 0x1F) * 255 / 31;
                int b = ((pixel >> 10) & 0x1F) * 255 / 31;
                int a = (pixel & 0x8000) != 0 ? 255 : 0;

                output[dst + 0] = (byte)b;
                output[dst + 1] = (byte)g;
                output[dst + 2] = (byte)r;
                output[dst + 3] = (byte)a;
            }
        }

        public static void Indexed8ToBgra32(byte[] input, byte[] clut, byte[] output, int pixels)
        {
            if (clut == null || input == null) return;

            for (int i = 0; i < pixels; i++)
            {
                if (i >= input.Length) break;
                int clutIndex = input[i] * 4;
                int dst = i * 4;

                if (clutIndex < clut.Length - 3)
                {
                    output[dst + 0] = clut[clutIndex + 2]; // B
                    output[dst + 1] = clut[clutIndex + 1]; // G
                    output[dst + 2] = clut[clutIndex + 0]; // R
                    output[dst + 3] = clut[clutIndex + 3]; // A
                }
                else
                {
                    output[dst + 0] = 0; // B
                    output[dst + 1] = 0; // G
                    output[dst + 2] = 0; // R
                    output[dst + 3] = 0; // A
                }
            }
        }

        public static void Indexed4ToBgra32(byte[] input, byte[] clut, byte[] output, int pixels)
        {
            if (clut == null || input == null) return;

            for (int i = 0; i < pixels; i++)
            {
                int inputByteIndex = i / 2;
                if (inputByteIndex >= input.Length) break;

                int inputByte = input[inputByteIndex];
                int shift = (i % 2) == 0 ? 0 : 4;
                int clutPaletteIndex = (inputByte >> shift) & 0x0F;
                int clutLookupIndex = clutPaletteIndex * 4;
                int dst = i * 4;

                if (clutLookupIndex < clut.Length - 3)
                {
                    output[dst + 0] = clut[clutLookupIndex + 2]; // B
                    output[dst + 1] = clut[clutLookupIndex + 1]; // G
                    output[dst + 2] = clut[clutLookupIndex + 0]; // R
                    output[dst + 3] = clut[clutLookupIndex + 3]; // A
                }
                else
                {
                    output[dst + 0] = 0; // B
                    output[dst + 1] = 0; // G
                    output[dst + 2] = 0; // R
                    output[dst + 3] = 0; // A
                }
            }
        }
    }
}