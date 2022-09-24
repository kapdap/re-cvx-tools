using System;

namespace RDXplorer.Models.RDX
{
    public class ItemModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<int> Unknown1 { get; set; } = new DataEntryModel<int>();
        public DataEntryModel<int> Type { get; set; } = new DataEntryModel<int>();
        public DataEntryModel<int> Unknown3 { get; set; } = new DataEntryModel<int>();
        public DataEntryModel<float> X { get; set; } = new DataEntryModel<float>();
        public DataEntryModel<float> Y { get; set; } = new DataEntryModel<float>();
        public DataEntryModel<float> Z { get; set; } = new DataEntryModel<float>();
        public DataEntryModel<short> XRot { get; set; } = new DataEntryModel<short>();
        public DataEntryModel<short> YRot { get; set; } = new DataEntryModel<short>();
        public DataEntryModel<int> ZRot { get; set; } = new DataEntryModel<int>();
        public DataEntryModel<int> Unknown7 { get; set; } = new DataEntryModel<int>();
    }
}