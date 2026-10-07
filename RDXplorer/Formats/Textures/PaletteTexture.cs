using RDXplorer.Formats.PVR;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RDXplorer.Formats.Textures
{
    /// <summary>
    /// Preview for palette-only blocks (Dreamcast PPVP).
    /// Renders the entries as a color strip.
    /// </summary>
    public sealed class PaletteTextureDocument : ITextureDocument
    {
        public IReadOnlyList<ITexturePicture> Pictures { get; }

        public PaletteTextureDocument(byte[] data)
        {
            if (!PvrPalette.TryParse(data, out int count, out byte[] rgba) || count <= 0)
                throw new InvalidDataException("Invalid palette.");

            Pictures = new ITexturePicture[] { new PaletteTexturePicture(count, rgba) };
        }
    }

    public sealed class PaletteTexturePicture : ITexturePicture
    {
        private readonly byte[] _rgba;

        public int Count { get; }
        public int Width { get; }
        public int Height { get; }

        public PaletteTexturePicture(int count, byte[] rgba)
        {
            Count = count;
            _rgba = rgba;
            Width = count <= 16 ? count : 16;
            Height = (count + Width - 1) / Width;
        }

        public string Description => $"Palette: {Count} colors";

        public BitmapSource Decode(bool disableAlpha = true)
        {
            byte[] bgra = new byte[Width * Height * 4];

            for (int i = 0; i < Count; i++)
            {
                bgra[i * 4 + 0] = _rgba[i * 4 + 2];
                bgra[i * 4 + 1] = _rgba[i * 4 + 1];
                bgra[i * 4 + 2] = _rgba[i * 4 + 0];
                bgra[i * 4 + 3] = _rgba[i * 4 + 3];
            }

            return BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgra32, null, bgra, Width * 4);
        }
    }
}
