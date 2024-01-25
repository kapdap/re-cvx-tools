using System;

namespace RDXplorer.Models.RDX
{
    public class TextureBlockModel
    {
        public IntPtr Offset { get; set; }

        public TextureTableModel Table { get; set; }

        public DataEntryModel<uint> Type { get; set; } = new();
        public DataEntryModel<uint> Size { get; set; } = new();
    }
}