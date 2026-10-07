namespace RDXplorer.Models.RDX
{
    public class ObjectModel : DataModel<ObjectModelFields> { }

    public class ObjectModelFields : IFieldsModel
    {
        public DataEntryModel<uint> Flg { get; set; } = new();
        public DataEntryModel<ushort> Id { get; set; } = new();
        public DataEntryModel<ushort> Type { get; set; } = new();
        public DataEntryModel<sbyte> FlrNo { get; set; } = new();
        public DataEntryModel<byte> MdlVer { get; set; } = new();
        public DataEntryModel<byte> WrkNo { get; set; } = new();
        public DataEntryModel<sbyte> Prm1 { get; set; } = new();
        public DataEntryModel<float> Px { get; set; } = new();
        public DataEntryModel<float> Py { get; set; } = new();
        public DataEntryModel<float> Pz { get; set; } = new();
        public DataEntryModel<short> Ax { get; set; } = new();
        public DataEntryModel<short> Az { get; set; } = new();
        public DataEntryModel<short> Ay { get; set; } = new();
        public DataEntryModel<short> Aspd { get; set; } = new();
        public DataEntryModel<byte> Hide0 { get; set; } = new();
        public DataEntryModel<byte> Hide1 { get; set; } = new();
        public DataEntryModel<byte> Hide2 { get; set; } = new();
        public DataEntryModel<byte> Hide3 { get; set; } = new();
    }
}
