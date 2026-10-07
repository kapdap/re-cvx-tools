using System.Collections.Generic;

namespace RDXplorer.Formats.PVR
{
    public class PvrDocument
    {
        public List<PvrPicture> Pictures { get; set; } = new();

        public PvrDocument(byte[] data)
            : this(data, null)
        {
        }

        public PvrDocument(byte[] data, byte[] paletteRgba)
        {
            Pictures.Add(new PvrPicture(data, paletteRgba));
        }
    }
}
