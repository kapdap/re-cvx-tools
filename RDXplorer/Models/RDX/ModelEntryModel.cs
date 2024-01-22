using System;

namespace RDXplorer.Models.RDX
{
    public class ModelEntryModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<uint> Type { get; set; } = new();
        public DataEntryModel<uint> Size { get; set; } = new();
        public DataEntryModel<uint> Data { get; set; } = new();

        public bool HasSize { get; set; } = false;
    }
}