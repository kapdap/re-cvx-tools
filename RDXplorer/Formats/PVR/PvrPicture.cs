using System;
using System.IO;

namespace RDXplorer.Formats.PVR
{
    /// <summary>
    /// Single Dreamcast PowerVR texture (GBIX + PVRT).
    /// </summary>
    public class PvrPicture
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public PvrPixelFormat PixelFormat { get; private set; }
        public PvrDataFormat DataFormat { get; private set; }

        public byte[] ImageData { get; private set; }
        public byte[] Pixels { get; private set; }

        public int TotalSize { get; private set; }

        public PvrPicture(byte[] data)
            : this(data, null)
        {
        }

        public PvrPicture(byte[] data, byte[] paletteRgba)
        {
            if (data == null)
                throw new InvalidDataException("Not a PVR texture. Data is null.");

            int offset = 0;

            if (HasMagic(data, offset, "GBIX"))
                offset += 16;

            if (!HasMagic(data, offset, "PVRT"))
                throw new InvalidDataException("Not a PVR texture. Invalid magic code.");

            if (data.Length - offset < 16)
                throw new InvalidDataException("Truncated PVRT header.");

            int size = BitConverter.ToInt32(data, offset + 4);
            PixelFormat = (PvrPixelFormat)data[offset + 8];
            DataFormat = (PvrDataFormat)data[offset + 9];
            Width = BitConverter.ToUInt16(data, offset + 12);
            Height = BitConverter.ToUInt16(data, offset + 14);

            if (Width <= 0 || Height <= 0)
                throw new InvalidDataException($"Invalid PVRT dimensions {Width}x{Height}.");

            if (size < 8)
                throw new InvalidDataException("Invalid PVRT data size.");

            int pixLen = size - 8;
            if (data.Length - (offset + 16) < pixLen)
                throw new InvalidDataException("Truncated PVRT blob.");

            byte[] pixdata = new byte[pixLen];
            Buffer.BlockCopy(data, offset + 16, pixdata, 0, pixLen);
            ImageData = pixdata;

            Pixels = DecodePixels(pixdata, Width, Height, PixelFormat, DataFormat, paletteRgba);

            TotalSize = (offset - 0) + 8 + size;
        }

        private static byte[] DecodePixels(byte[] pixdata, int width, int height,
            PvrPixelFormat pixfmt, PvrDataFormat datafmt, byte[] paletteRgba)
        {
            switch (datafmt)
            {
                case PvrDataFormat.Twiddled:
                    return DecodeWords(Detwiddle16(pixdata, width, height), pixfmt);

                case PvrDataFormat.RectTwiddled:
                    return DecodeWords(DetwiddleRect16(pixdata, width, height), pixfmt);

                case PvrDataFormat.TwiddledMipmap:
                    {
                        int baseBytes = width * height * 2;
                        if (pixdata.Length < baseBytes)
                            throw new InvalidDataException("Truncated mipmapped PVRT blob.");
                        byte[] baseLevel = new byte[baseBytes];
                        Buffer.BlockCopy(pixdata, pixdata.Length - baseBytes, baseLevel, 0, baseBytes);
                        return DecodeWords(Detwiddle16(baseLevel, width, height), pixfmt);
                    }

                case PvrDataFormat.Rectangle:
                    {
                        int n = width * height;
                        if (pixdata.Length < n * 2)
                            throw new InvalidDataException("Truncated rectangle PVRT blob.");
                        ushort[] words = new ushort[n];
                        for (int i = 0; i < n; i++)
                            words[i] = BitConverter.ToUInt16(pixdata, i * 2);
                        return DecodeWords(words, pixfmt);
                    }

                case PvrDataFormat.Vq:
                case PvrDataFormat.SmallVq:
                    return DecodeVq(pixdata, width, height, pixfmt);

                case PvrDataFormat.Pal4:
                    {
                        int[] indices = Detwiddle4(pixdata, width, height);
                        return ApplyPalette(indices, paletteRgba, pixfmt, width * height);
                    }

                case PvrDataFormat.Pal8:
                    {
                        int[] indices = Detwiddle8(pixdata, width, height);
                        return ApplyPalette(indices, paletteRgba, pixfmt, width * height);
                    }

                default:
                    throw new InvalidDataException($"Unsupported Dreamcast data format 0x{(byte)datafmt:X2}.");
            }
        }

        public static void DecodeTexel(ushort v, PvrPixelFormat pixfmt,
            out byte r, out byte g, out byte b, out byte a)
        {
            switch (pixfmt)
            {
                case PvrPixelFormat.Argb1555:
                    r = (byte)(((v >> 10) & 0x1F) * 255 / 31);
                    g = (byte)(((v >> 5) & 0x1F) * 255 / 31);
                    b = (byte)((v & 0x1F) * 255 / 31);
                    a = (v & 0x8000) != 0 ? (byte)255 : (byte)0;
                    return;

                case PvrPixelFormat.Rgb565:
                    r = (byte)(((v >> 11) & 0x1F) * 255 / 31);
                    g = (byte)(((v >> 5) & 0x3F) * 255 / 63);
                    b = (byte)((v & 0x1F) * 255 / 31);
                    a = 255;
                    return;

                case PvrPixelFormat.Argb4444:
                    a = (byte)(((v >> 12) & 0x0F) * 17);
                    r = (byte)(((v >> 8) & 0x0F) * 17);
                    g = (byte)(((v >> 4) & 0x0F) * 17);
                    b = (byte)((v & 0x0F) * 17);
                    return;

                default:
                    throw new InvalidDataException($"Unsupported Dreamcast pixel format 0x{(byte)pixfmt:X2}.");
            }
        }

        /// <summary>
        /// Morton index with y in even bit positions, x in odd positions.
        /// </summary>
        public static int Morton(int x, int y)
        {
            int output = 0;
            int shift = 0;
            while (x != 0 || y != 0)
            {
                output |= (y & 1) << shift;
                output |= (x & 1) << (shift + 1);
                x >>= 1;
                y >>= 1;
                shift += 2;
            }
            return output;
        }

        private static ushort[] Detwiddle16(byte[] src, int width, int height)
        {
            int n = width * height;
            if (src.Length < n * 2)
                throw new InvalidDataException("Truncated twiddled PVRT blob.");

            ushort[] words = new ushort[n];
            for (int i = 0; i < n; i++)
                words[i] = BitConverter.ToUInt16(src, i * 2);

            ushort[] dst = new ushort[n];
            for (int y = 0; y < height; y++)
            {
                int baseIndex = y * width;
                for (int x = 0; x < width; x++)
                    dst[baseIndex + x] = words[Morton(x, y)];
            }
            return dst;
        }

        private static int[] Detwiddle8(byte[] src, int width, int height)
        {
            int n = width * height;
            if (src.Length < n)
                throw new InvalidDataException("Truncated paletted PVRT blob.");

            int[] dst = new int[n];
            for (int y = 0; y < height; y++)
            {
                int baseIndex = y * width;
                for (int x = 0; x < width; x++)
                    dst[baseIndex + x] = src[Morton(x, y)];
            }
            return dst;
        }

        private static int[] Detwiddle4(byte[] src, int width, int height)
        {
            int n = width * height;
            if (src.Length < (n + 1) / 2)
                throw new InvalidDataException("Truncated 4-bit paletted PVRT blob.");

            int[] dst = new int[n];
            for (int y = 0; y < height; y++)
            {
                int baseIndex = y * width;
                for (int x = 0; x < width; x++)
                {
                    int mi = Morton(x, y);
                    byte b = src[mi / 2];
                    dst[baseIndex + x] = (mi % 2 == 0) ? (b & 0x0F) : ((b >> 4) & 0x0F);
                }
            }
            return dst;
        }

        private static ushort[] DetwiddleRect16(byte[] src, int width, int height)
        {
            int side = Math.Min(width, height);
            int longSide = Math.Max(width, height);
            if (side <= 0 || longSide % side != 0)
                throw new InvalidDataException($"Bad rectangular size {width}x{height}.");

            int tiles = longSide / side;
            if (src.Length < tiles * side * side * 2)
                throw new InvalidDataException("Truncated rectangular PVRT blob.");

            ushort[] dst = new ushort[width * height];
            bool wide = width >= height;

            for (int t = 0; t < tiles; t++)
            {
                int tileBase = t * side * side * 2;
                for (int y = 0; y < side; y++)
                {
                    for (int x = 0; x < side; x++)
                    {
                        int mi = Morton(x, y);
                        ushort v = BitConverter.ToUInt16(src, tileBase + mi * 2);
                        int dx = wide ? t * side + x : x;
                        int dy = wide ? y : t * side + y;
                        dst[dy * width + dx] = v;
                    }
                }
            }
            return dst;
        }

        private static byte[] DecodeWords(ushort[] words, PvrPixelFormat pixfmt)
        {
            byte[] output = new byte[words.Length * 4];
            for (int i = 0; i < words.Length; i++)
            {
                DecodeTexel(words[i], pixfmt, out byte r, out byte g, out byte b, out byte a);
                output[i * 4 + 0] = r;
                output[i * 4 + 1] = g;
                output[i * 4 + 2] = b;
                output[i * 4 + 3] = a;
            }
            return output;
        }

        private static byte[] DecodeVq(byte[] pixdata, int width, int height, PvrPixelFormat pixfmt)
        {
            int nidx = (width / 2) * (height / 2);
            int codebookSize = pixdata.Length - nidx;

            if (codebookSize <= 0 || codebookSize % 8 != 0)
                throw new InvalidDataException($"Bad VQ codebook size {codebookSize}.");

            int halfW = width / 2;
            int halfH = height / 2;

            int[] indices = new int[nidx];
            for (int y = 0; y < halfH; y++)
            {
                int baseIndex = y * halfW;
                for (int x = 0; x < halfW; x++)
                    indices[baseIndex + x] = pixdata[codebookSize + Morton(x, y)];
            }

            int entries = codebookSize / 8;
            ushort[] words = new ushort[width * height];

            for (int by = 0; by < halfH; by++)
            {
                for (int bx = 0; bx < halfW; bx++)
                {
                    int entry = indices[by * halfW + bx] % entries;
                    int codeBase = entry * 8;
                    ushort t0 = BitConverter.ToUInt16(pixdata, codeBase);
                    ushort t1 = BitConverter.ToUInt16(pixdata, codeBase + 2);
                    ushort t2 = BitConverter.ToUInt16(pixdata, codeBase + 4);
                    ushort t3 = BitConverter.ToUInt16(pixdata, codeBase + 6);

                    int dx = bx * 2;
                    int row0 = (by * 2) * width + dx;
                    int row1 = (by * 2 + 1) * width + dx;
                    words[row0] = t0;
                    words[row0 + 1] = t2;
                    words[row1] = t1;
                    words[row1 + 1] = t3;
                }
            }

            return DecodeWords(words, pixfmt);
        }

        private static byte[] ApplyPalette(int[] indices, byte[] paletteRgba, PvrPixelFormat pixfmt, int count)
        {
            byte[] output = new byte[count * 4];

            for (int i = 0; i < count; i++)
            {
                int ci = i < indices.Length ? indices[i] : 0;

                if (paletteRgba != null && ci * 4 + 3 < paletteRgba.Length)
                {
                    output[i * 4 + 0] = paletteRgba[ci * 4 + 0];
                    output[i * 4 + 1] = paletteRgba[ci * 4 + 1];
                    output[i * 4 + 2] = paletteRgba[ci * 4 + 2];
                    output[i * 4 + 3] = paletteRgba[ci * 4 + 3];
                }
                else
                {
                    ushort v = (ushort)((ci * 17) & 0xFFFF);
                    DecodeTexel(v, PvrPixelFormat.Argb4444, out byte r, out byte g, out byte b, out byte a);
                    output[i * 4 + 0] = r;
                    output[i * 4 + 1] = g;
                    output[i * 4 + 2] = b;
                    output[i * 4 + 3] = a;
                }
            }

            return output;
        }

        private static bool HasMagic(byte[] data, int offset, string magic)
        {
            if (offset < 0 || data.Length - offset < 4)
                return false;
            return data[offset] == magic[0] && data[offset + 1] == magic[1] &&
                   data[offset + 2] == magic[2] && data[offset + 3] == magic[3];
        }
    }
}
