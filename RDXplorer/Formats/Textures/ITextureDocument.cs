using System.Collections.Generic;

namespace RDXplorer.Formats.Textures
{
    /// <summary>
    /// A decoded texture block. Most blocks hold one picture;
    /// TIM2 blocks can hold several.
    /// </summary>
    public interface ITextureDocument
    {
        IReadOnlyList<ITexturePicture> Pictures { get; }
    }
}
