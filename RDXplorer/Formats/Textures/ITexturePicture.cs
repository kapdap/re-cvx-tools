using System.Windows.Media.Imaging;

namespace RDXplorer.Formats.Textures
{
    /// <summary>
    /// A single viewable texture image, regardless of source format
    /// (TIM2 pictures, PVR textures, palette previews).
    /// </summary>
    public interface ITexturePicture
    {
        int Width { get; }
        int Height { get; }

        string Description { get; }

        BitmapSource Decode(bool disableAlpha = true);
    }
}
