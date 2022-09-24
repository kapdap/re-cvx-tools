using System;

namespace RDXplorer.Models.RDX
{
    public class ItemModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<int> Unknown1 { get; set; } = new();
        public DataEntryModel<int> Type { get; set; } = new();
        public DataEntryModel<int> Unknown3 { get; set; } = new();
        public DataEntryModel<float> X { get; set; } = new();
        public DataEntryModel<float> Y { get; set; } = new();
        public DataEntryModel<float> Z { get; set; } = new();
        public DataEntryModel<short> XRot { get; set; } = new();
        public DataEntryModel<short> YRot { get; set; } = new();
        public DataEntryModel<int> ZRot { get; set; } = new();
        public DataEntryModel<int> Unknown7 { get; set; } = new();
    }
}