using System;
using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    public class TextureTableModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<uint> Pointer { get; set; } = new();
        public List<TextureBlockModel> Blocks { get; set; } = new();
        public uint Size { get; set; }

        public TextureTableModel()
        {
            Pointer.IsPointer = true;
        }
    }
}