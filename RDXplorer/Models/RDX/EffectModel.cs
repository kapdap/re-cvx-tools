namespace RDXplorer.Models.RDX
{
    // On-disk layout: EF_WRK (types.h) — 0x44 bytes.
    // NOTE: file order is Ay then Ax (not Ax then Ay).
    public class EffectModel : DataModel<EffectModelFields> { }

    public class EffectModelFields : IFieldsModel
    {
        public DataEntryModel<uint> Flg { get; set; } = new();
        public DataEntryModel<ushort> Id { get; set; } = new();
        public DataEntryModel<ushort> Type { get; set; } = new();
        public DataEntryModel<short> FlrNo { get; set; } = new();
        public DataEntryModel<ushort> MdlVer { get; set; } = new();
        public DataEntryModel<float> Px { get; set; } = new();
        public DataEntryModel<float> Py { get; set; } = new();
        public DataEntryModel<float> Pz { get; set; } = new();
        public DataEntryModel<float> Sx { get; set; } = new();
        public DataEntryModel<float> Sy { get; set; } = new();
        public DataEntryModel<float> Sz { get; set; } = new();
        public DataEntryModel<short> Ay { get; set; } = new();
        public DataEntryModel<short> Ax { get; set; } = new();
        public DataEntryModel<int> LkFlg { get; set; } = new();
        public DataEntryModel<int> LkNo { get; set; } = new();
        public DataEntryModel<int> LkOno { get; set; } = new();
        public DataEntryModel<float> Lx { get; set; } = new();
        public DataEntryModel<float> Ly { get; set; } = new();
        public DataEntryModel<float> Lz { get; set; } = new();
        public DataEntryModel<int> Param { get; set; } = new();
    }
}
