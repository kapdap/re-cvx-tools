using System;
using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    public class MotionTableModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<uint> Pointer { get; set; } = new();
        public List<MotionBlockModel> Blocks { get; set; } = new();
        public uint Size { get; set; }

        public MotionTableModel()
        {
            Pointer.IsPointer = true;
        }
    }
}