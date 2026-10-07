using RDXplorer.Formats.PVR;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace RDXplorer.Formats.Textures
{
    public sealed class PvrTextureDocument : ITextureDocument
    {
        public IReadOnlyList<ITexturePicture> Pictures { get; }

        public PvrTextureDocument(byte[] data, byte[] paletteRgba = null)
        {
            PvrDocument document = new(data, paletteRgba);
            Pictures = new ITexturePicture[] { new PvrTexturePicture(document.Pictures[0]) };
        }
    }

    public sealed class PvrTexturePicture : ITexturePicture
    {
        private readonly PvrPicture _picture;

        public PvrTexturePicture(PvrPicture picture) =>
            _picture = picture;

        public int Width => _picture.Width;
        public int Height => _picture.Height;

        public string Description =>
            $"({Utilities.FormatFileSize(_picture.Pixels.Length)}) ({_picture.PixelFormat}) ({_picture.DataFormat})";

        public BitmapSource Decode(bool disableAlpha = true) =>
            PvrConverter.Decode(_picture, disableAlpha);
    }
}
