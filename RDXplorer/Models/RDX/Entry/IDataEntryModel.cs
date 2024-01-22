using System;

namespace RDXplorer.Models.RDX
{
    public interface IDataEntryModel
    {
        public IntPtr Offset { get; }
        public long Size { get; }
        public byte[] Data { get; }
        public string Text { get; }

        public bool IsPointer { get; set; }
        public bool IsText { get; set; }
    }
}