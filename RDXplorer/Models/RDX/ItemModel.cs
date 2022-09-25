using System;

namespace RDXplorer.Models.RDX
{
    public class ItemModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<byte> Unknown1 { get; set; } = new();
        public DataEntryModel<byte> Unknown2 { get; set; } = new();
        public DataEntryModel<byte> Unknown3 { get; set; } = new();
        public DataEntryModel<byte> Unknown4 { get; set; } = new();
        public DataEntryModel<int> Type { get; set; } = new();
        public DataEntryModel<int> Unknown5 { get; set; } = new();
        public DataEntryModel<float> X { get; set; } = new();
        public DataEntryModel<float> Y { get; set; } = new();
        public DataEntryModel<float> Z { get; set; } = new();
        public DataEntryModel<short> XRot { get; set; } = new();
        public DataEntryModel<short> YRot { get; set; } = new();
        public DataEntryModel<short> ZRot { get; set; } = new();
        public DataEntryModel<short> Unknown6 { get; set; } = new();
        public DataEntryModel<byte> Unknown7 { get; set; } = new();
        public DataEntryModel<byte> Unknown8 { get; set; } = new();
        public DataEntryModel<byte> Unknown9 { get; set; } = new();
        public DataEntryModel<byte> Unknown10 { get; set; } = new();
    }
}