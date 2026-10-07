using System;

namespace RDXplorer.Formats.PVR
{
    /// <summary>
    /// Dreamcast PowerVR palette blob (PVPL) stored in RDX PPVP blocks.
    /// </summary>
    public static class PvrPalette
    {
        public static bool TryParse(byte[] data, out int count, out byte[] rgba)
        {
            return TryParse(data, PvrPixelFormat.Argb4444, out count, out rgba);
        }

        public static bool TryParse(byte[] data, PvrPixelFormat pixelFormat, out int count, out byte[] rgba)
        {
            count = 0;
            rgba = null;

            if (data == null || data.Length < 16)
                return false;

            if (data[0] != (byte)'P' || data[1] != (byte)'V' || data[2] != (byte)'P' || data[3] != (byte)'L')
                return false;

            int size = BitConverter.ToInt32(data, 4);
            if (size < 8)
                return false;

            int fieldCount = BitConverter.ToUInt16(data, 14);
            int available = Math.Max(0, size - 8) / 2;

            int present = (data.Length - 16) / 2;
            int n = Math.Min(available, present);

            if (fieldCount >= 1 && fieldCount <= 256)
                n = Math.Min(n, fieldCount);

            if (n <= 0 || n > 256)
                return false;

            rgba = new byte[n * 4];

            for (int i = 0; i < n; i++)
            {
                ushort v = BitConverter.ToUInt16(data, 16 + i * 2);
                PvrPicture.DecodeTexel(v, pixelFormat, out byte r, out byte g, out byte b, out byte a);
                rgba[i * 4 + 0] = r;
                rgba[i * 4 + 1] = g;
                rgba[i * 4 + 2] = b;
                rgba[i * 4 + 3] = a;
            }

            count = n;
            return true;
        }
    }
}
