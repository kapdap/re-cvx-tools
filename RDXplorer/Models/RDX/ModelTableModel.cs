using System;
using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    public class ModelTableModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<uint> Pointer { get; set; } = new();
        public List<ModelBlockModel> Blocks { get; set; } = new();
        public uint Size { get; set; }

        public ModelTableModel()
        {
            Pointer.IsPointer = true;
        }
    }
}