using System;

namespace RDXplorer.Models.RDX
{
    public class EnemyModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<int> Unknown0 { get; set; } = new();
        public DataEntryModel<short> Type { get; set; } = new();
        public DataEntryModel<short> Unknown1 { get; set; } = new();
        public DataEntryModel<byte> Unknown2 { get; set; } = new();
        public DataEntryModel<byte> Unknown3 { get; set; } = new();
        public DataEntryModel<byte> Slot { get; set; } = new();
        public DataEntryModel<byte> Unknown4 { get; set; } = new();
        public DataEntryModel<float> X { get; set; } = new();
        public DataEntryModel<float> Y { get; set; } = new();
        public DataEntryModel<float> Z { get; set; } = new();
        public DataEntryModel<int> Unknown5 { get; set; } = new();
        public DataEntryModel<short> Rotation { get; set; } = new();
        public DataEntryModel<short> Unknown6 { get; set; } = new();
        public DataEntryModel<int> Unknown7 { get; set; } = new();
    }
}