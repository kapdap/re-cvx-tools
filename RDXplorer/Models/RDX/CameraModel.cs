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
        public DataEntryModel<int> Offset1 { get; set; } = new();
        public DataEntryModel<float> Unknown6 { get; set; } = new();
        public DataEntryModel<float> Unknown7 { get; set; } = new();
        public DataEntryModel<float> Unknown8 { get; set; } = new();
        public DataEntryModel<float> Unknown9 { get; set; } = new();
        public DataEntryModel<float> Unknown10 { get; set; } = new();
        public DataEntryModel<float> Unknown11 { get; set; } = new();
        public DataEntryModel<float> Unknown12 { get; set; } = new();
        public DataEntryModel<float> X { get; set; } = new();
        public DataEntryModel<float> Y { get; set; } = new();
        public DataEntryModel<float> Z { get; set; } = new();
        public DataEntryModel<float> Unknown16 { get; set; } = new();
        public DataEntryModel<float> Unknown17 { get; set; } = new();
        public DataEntryModel<float> Unknown18 { get; set; } = new();
        public DataEntryModel<float> Unknown19 { get; set; } = new();
        public DataEntryModel<float> Unknown20 { get; set; } = new();
        public DataEntryModel<float> Unknown21 { get; set; } = new();
        public DataEntryModel<float> Unknown22 { get; set; } = new();
        public DataEntryModel<float> Unknown23 { get; set; } = new();
        public DataEntryModel<float> Unknown24 { get; set; } = new();
        public DataEntryModel<int> XRotation { get; set; } = new();
        public DataEntryModel<int> YRotation { get; set; } = new();
        public DataEntryModel<int> ZRotation { get; set; } = new();
        public DataEntryModel<float> Unknown28 { get; set; } = new();
        public DataEntryModel<float> Unknown29 { get; set; } = new();
        public DataEntryModel<float> Unknown30 { get; set; } = new();
        public DataEntryModel<float> Unknown31 { get; set; } = new();
        public DataEntryModel<int> Unknown32 { get; set; } = new();
        public DataEntryModel<int> Perspective { get; set; } = new();
        public DataEntryModel<int> Unknown34 { get; set; } = new();
        public DataEntryModel<int> Unknown35 { get; set; } = new();
        public DataEntryModel<int> Unknown36 { get; set; } = new();
    }
}