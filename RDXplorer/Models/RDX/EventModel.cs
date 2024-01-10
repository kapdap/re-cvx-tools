using System;

namespace RDXplorer.Models.RDX
{
    public class EventModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<byte> Unknown1 { get; set; } = new();
        public DataEntryModel<byte> Unknown2 { get; set; } = new();
        public DataEntryModel<byte> Unknown3 { get; set; } = new();
        public DataEntryModel<byte> Unknown4 { get; set; } = new();
        public DataEntryModel<int> Unknown5 { get; set; } = new();
        public DataEntryModel<float> X { get; set; } = new();
        public DataEntryModel<float> Y { get; set; } = new();
        public DataEntryModel<float> Z { get; set; } = new();
        public DataEntryModel<float> Width { get; set; } = new();
        public DataEntryModel<float> Height { get; set; } = new();
        public DataEntryModel<float> Length { get; set; } = new();
        public DataEntryModel<byte> Unknown12 { get; set; } = new();
        public DataEntryModel<byte> Unknown13 { get; set; } = new();
        public DataEntryModel<byte> Unknown14 { get; set; } = new();
        public DataEntryModel<byte> Unknown15 { get; set; } = new();
    }
}