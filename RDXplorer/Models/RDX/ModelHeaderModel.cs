using System;
using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    public class ModelHeaderModel
    {
        public IntPtr Offset { get; set; }

        public HeaderEntryModel Pointer { get; set; } = new("Pointer");
        public List<ModelEntryModel> Entry { get; set; } = new();
        public uint Size { get; set; }
    }
}