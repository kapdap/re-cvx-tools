using System;

namespace RDXplorer.Models.RDX
{
    public class CameraModel
    {
        public IntPtr Offset { get; set; }

        public DataEntryModel<byte> Unknown1 { get; set; } = new();
        public DataEntryModel<byte> Unknown2 { get; set; } = new();
        public DataEntryModel<byte> Unknown3 { get; set; } = new();
        public DataEntryModel<byte> Unknown4 { get; set; } = new();
        public DataEntryModel<int> Unknown5 { get; set; } = new();
        public DataEntryModel<float> Unknown6 { get; set; } = new();
        public DataEntryModel<float> Unknown7 { get; set; } = new();
        public DataEntryModel<float> Unknown8 { get; set; } = new();
        public DataEntryModel<float> Unknown9 { get; set; } = new();
        public DataEntryModel<float> Unknown10 { get; set; } = new();
    }
}