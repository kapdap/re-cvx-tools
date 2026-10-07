namespace RDXplorer.Models.RDX
{
    public class EventLightModel : DataModel<EventLightModelFields> { }

    public class EventLightModelFields : IFieldsModel
    {
        public DataEntryModel<uint> Flg { get; set; } = new();
        public DataEntryModel<uint> Type { get; set; } = new();
        public DataEntryModel<int> Aspd { get; set; } = new();
        public DataEntryModel<int> LkFlg { get; set; } = new();
        public DataEntryModel<int> LkNo { get; set; } = new();
        public DataEntryModel<int> LkOno { get; set; } = new();
        public DataEntryModel<int> Lsrc { get; set; } = new();
        public DataEntryModel<float> Px { get; set; } = new();
        public DataEntryModel<float> Py { get; set; } = new();
        public DataEntryModel<float> Pz { get; set; } = new();
        public DataEntryModel<float> Lx { get; set; } = new();
        public DataEntryModel<float> Ly { get; set; } = new();
        public DataEntryModel<float> Lz { get; set; } = new();
        public DataEntryModel<float> Vx { get; set; } = new();
        public DataEntryModel<float> Vy { get; set; } = new();
        public DataEntryModel<float> Vz { get; set; } = new();
        public DataEntryModel<float> Spc { get; set; } = new();
        public DataEntryModel<float> Dif { get; set; } = new();
        public DataEntryModel<float> Amb { get; set; } = new();
        public DataEntryModel<float> R { get; set; } = new();
        public DataEntryModel<float> G { get; set; } = new();
        public DataEntryModel<float> B { get; set; } = new();
        public DataEntryModel<float> Nr { get; set; } = new();
        public DataEntryModel<float> Fr { get; set; } = new();
        public DataEntryModel<int> Iang { get; set; } = new();
        public DataEntryModel<int> Oang { get; set; } = new();
        public DataEntryModel<int> Ax { get; set; } = new();
        public DataEntryModel<int> Ay { get; set; } = new();
        public DataEntryModel<int> Az { get; set; } = new();
        public DataEntryModel<uint> Mode { get; set; } = new();
        public DataEntryModel<int> Ct0 { get; set; } = new();
        public DataEntryModel<int> Ct1 { get; set; } = new();
        public DataEntryModel<int> Ct2 { get; set; } = new();
        public DataEntryModel<int> Ct3 { get; set; } = new();
        public DataEntryModel<float> Wpx { get; set; } = new();
        public DataEntryModel<float> Wpy { get; set; } = new();
        public DataEntryModel<float> Wpz { get; set; } = new();
        public DataEntryModel<float> Wvx { get; set; } = new();
        public DataEntryModel<float> Wvy { get; set; } = new();
        public DataEntryModel<float> Wvz { get; set; } = new();
        public DataEntryModel<float> Wspc { get; set; } = new();
        public DataEntryModel<float> Wdif { get; set; } = new();
        public DataEntryModel<float> Wamb { get; set; } = new();
        public DataEntryModel<float> Wr { get; set; } = new();
        public DataEntryModel<float> Wg { get; set; } = new();
        public DataEntryModel<float> Wb { get; set; } = new();
        public DataEntryModel<float> Wnr { get; set; } = new();
        public DataEntryModel<float> Wfr { get; set; } = new();
        public DataEntryModel<int> Wiang { get; set; } = new();
        public DataEntryModel<int> Woang { get; set; } = new();
        public DataEntryModel<int> Wax { get; set; } = new();
        public DataEntryModel<int> Way { get; set; } = new();
        public DataEntryModel<int> Waz { get; set; } = new();
        public DataEntryModel<uint> Lkwkp { get; set; } = new();
        public DataEntryModel<uint> Exp { get; set; } = new();
        public DataEntryModel<uint> Light { get; set; } = new();
    }
}
