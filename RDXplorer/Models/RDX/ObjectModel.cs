﻿namespace RDXplorer.Models.RDX
{
    public class ObjectModel : DataModel<ObjectModelFields> { }

    public class ObjectModelFields : IFieldsModel
    {
        public DataEntryModel<byte> Visible { get; set; } = new();
        public DataEntryModel<byte> Unknown1 { get; set; } = new();
        public DataEntryModel<byte> Unknown2 { get; set; } = new();
        public DataEntryModel<byte> Unknown3 { get; set; } = new();
        public DataEntryModel<short> Type { get; set; } = new();
        public DataEntryModel<byte> Unknown4 { get; set; } = new();
        public DataEntryModel<byte> Unknown5 { get; set; } = new();
        public DataEntryModel<byte> Unknown6 { get; set; } = new();
        public DataEntryModel<byte> Unknown7 { get; set; } = new();
        public DataEntryModel<byte> Unknown8 { get; set; } = new();
        public DataEntryModel<byte> Unknown9 { get; set; } = new();
        public DataEntryModel<float> X { get; set; } = new();
        public DataEntryModel<float> Y { get; set; } = new();
        public DataEntryModel<float> Z { get; set; } = new();
        public DataEntryModel<short> XRot { get; set; } = new();
        public DataEntryModel<short> YRot { get; set; } = new();
        public DataEntryModel<short> ZRot { get; set; } = new();
        public DataEntryModel<short> Unknown10 { get; set; } = new();
        public DataEntryModel<byte> Unknown11 { get; set; } = new();
        public DataEntryModel<byte> Unknown12 { get; set; } = new();
        public DataEntryModel<byte> Unknown13 { get; set; } = new();
        public DataEntryModel<byte> Unknown14 { get; set; } = new();
    }
}