namespace RDXplorer.Models.RDX
{
    // On-disk layout: ATR_WORK (types.h) — 0x24 bytes.
    // Door: Prm0 = StgNo, Prm1 = RomNo, Prm2 = PosNo, Prm3 = DorTp (see hitchk.c).
    public class AOTModel : DataModel<AOTModelFields> { }

    public class AOTModelFields : IFieldsModel
    {
        public DataEntryModel<byte> Flg { get; set; } = new();
        public DataEntryModel<byte> Type { get; set; } = new();
        public DataEntryModel<byte> Id { get; set; } = new();
        public DataEntryModel<sbyte> FlrNo { get; set; } = new();
        public DataEntryModel<uint> Attr { get; set; } = new();
        public DataEntryModel<float> Px { get; set; } = new();
        public DataEntryModel<float> Py { get; set; } = new();
        public DataEntryModel<float> Pz { get; set; } = new();
        public DataEntryModel<float> W { get; set; } = new();
        public DataEntryModel<float> H { get; set; } = new();
        public DataEntryModel<float> D { get; set; } = new();
        public DataEntryModel<byte> Prm0 { get; set; } = new();
        public DataEntryModel<byte> Prm1 { get; set; } = new();
        public DataEntryModel<byte> Prm2 { get; set; } = new();
        public DataEntryModel<byte> Prm3 { get; set; } = new();
    }
}
