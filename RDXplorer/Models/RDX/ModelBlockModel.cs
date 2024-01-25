using System;

namespace RDXplorer.Models.RDX
{
    public class ModelBlockModel
    {
        public IntPtr Offset { get; set; }

        public ModelTableModel Table { get; set; }

        public DataEntryModel<uint> Type { get; set; } = new();
        public DataEntryModel<uint> Size { get; set; } = new();

        public bool HasSize { get; set; } = false;
    }
}