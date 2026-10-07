using RDXplorer.Formats.PVR;
using RDXplorer.Models.RDX;

namespace RDXplorer.Formats.Textures
{
    /// <summary>
    /// Loads any texture-table block into the generic texture interface.
    /// Owns all format dispatch and palette resolution, so viewers never
    /// touch TIM2/PVR/PVP specifics.
    /// </summary>
    public static class TextureLoader
    {
        public static bool TryLoad(TextureBlockModel block, out ITextureDocument document)
        {
            document = null;

            if (block == null)
                return false;

            string type = block.Fields.Type.Text ?? string.Empty;
            byte[] data = block.Fields.Data.Data;

            if (type == "TIM2")
            {
                document = new Tim2TextureDocument(data);
                return true;
            }

            if (type.Contains("PVR"))
            {
                document = new PvrTextureDocument(data, ResolvePalette(block));
                return true;
            }

            if (type.Contains("PVP"))
            {
                document = new PaletteTextureDocument(data);
                return true;
            }

            return false;
        }

        public static bool TryGetPicture(TextureBlockModel block, out ITexturePicture picture)
        {
            picture = null;

            if (!TryLoad(block, out ITextureDocument document) || document.Pictures.Count == 0)
                return false;

            picture = document.Pictures[0];
            return true;
        }

        private static byte[] ResolvePalette(TextureBlockModel block)
        {
            if (block?.Table?.Blocks == null)
                return null;

            byte[] palette = null;
            int index = block.Table.Blocks.IndexOf(block);

            if (index < 0)
                index = block.Table.Blocks.Count - 1;

            for (int i = 0; i <= index && i < block.Table.Blocks.Count; i++)
            {
                TextureBlockModel candidate = block.Table.Blocks[i];

                if (candidate.Fields.Type.Text?.Contains("PVP") != true)
                    continue;

                if (PvrPalette.TryParse(candidate.Fields.Data.Data, out _, out byte[] rgba))
                    palette = rgba;
            }

            return palette;
        }
    }
}
