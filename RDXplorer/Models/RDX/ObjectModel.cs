using System;

namespace RDXplorer.Models.RDX
{
    public class ObjectModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<byte> Visible { get; set; } = new();
        public DataEntryModel<byte> Unknown1 { get; set; } = new();
        public DataEntryModel<byte> Unknown2 { get; set; } = new();
        public DataEntryModel<byte> Unknown3 { get; set; } = new();
        public DataEntryModel<short> Unknown4 { get; set; } = new();
        public DataEntryModel<short> Unknown5 { get; set; } = new();
        public DataEntryModel<int> Unknown6 { get; set; } = new();
        public DataEntryModel<float> X { get; set; } = new();
        public DataEntryModel<float> Y { get; set; } = new();
        public DataEntryModel<float> Z { get; set; } = new();
        public DataEntryModel<int> XRot { get; set; } = new();
        public DataEntryModel<int> YRot { get; set; } = new();
        public DataEntryModel<int> ZRot { get; set; } = new();
    }
}