using RDXplorer.Formats.TIM2;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace RDXplorer.Formats.Textures
{
    public sealed class Tim2TextureDocument : ITextureDocument
    {
        public IReadOnlyList<ITexturePicture> Pictures { get; }

        public Tim2TextureDocument(byte[] data)
        {
            Tim2Document document = new(data);
            Pictures = document.Pictures.Select(p => (ITexturePicture)new Tim2TexturePicture(p)).ToList();
        }
    }

    public sealed class Tim2TexturePicture : ITexturePicture
    {
        private readonly Tim2Picture _picture;

        public Tim2TexturePicture(Tim2Picture picture) =>
            _picture = picture;

        public int Width => _picture.Width;
        public int Height => _picture.Height;

        public string Description =>
            $"({Utilities.FormatFileSize(_picture.ImageSize)}) ({_picture.ImageColorType}) ({_picture.ClutColorType})";

        public BitmapSource Decode(bool disableAlpha = true) =>
            Tim2Converter.Decode(_picture, disableAlpha);
    }
}
