using System;

namespace RDXplorer.Models.RDX
{
    public class ScriptModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<uint> Position { get; set; } = new();
        public DataEntryModel<uint> Pointer { get; set; } = new();
        public DataEntryModel<byte> Data { get; set; } = new();

        public uint Size { get; set; }

        public ScriptModel()
        {
            Pointer.IsPointer = true;
        }
    }
}