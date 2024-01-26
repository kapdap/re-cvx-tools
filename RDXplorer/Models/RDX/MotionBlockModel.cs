using System;

namespace RDXplorer.Models.RDX
{
    public class MotionBlockModel
    {
        public IntPtr Offset { get; set; }

        public MotionTableModel Table { get; set; }

        public DataEntryModel<uint> Type { get; set; } = new();
        public DataEntryModel<uint> Size { get; set; } = new();
        public DataEntryModel<uint> Data { get; set; } = new();
    }
}