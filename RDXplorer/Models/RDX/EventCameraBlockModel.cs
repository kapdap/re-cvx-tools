namespace RDXplorer.Models.RDX
{
    public class EventCameraBlockModel : DataModel<EventCameraBlockModelFields>
    {
        public EventCameraModel Header { get; set; }
    }

    public class EventCameraBlockModelFields : IFieldsModel
    {
        public DataEntryModel<ushort> Flg { get; set; } = new();
        public DataEntryModel<short> Frame { get; set; } = new();
        public DataEntryModel<float> Px { get; set; } = new();
        public DataEntryModel<float> Py { get; set; } = new();
        public DataEntryModel<float> Pz { get; set; } = new();
        public DataEntryModel<short> Ax { get; set; } = new();
        public DataEntryModel<short> Ay { get; set; } = new();
        public DataEntryModel<short> Az { get; set; } = new();
        public DataEntryModel<short> Pers { get; set; } = new();
        public DataEntryModel<uint> HidObj0 { get; set; } = new();
        public DataEntryModel<uint> HidObj1 { get; set; } = new();
        public DataEntryModel<uint> HidObj2 { get; set; } = new();
        public DataEntryModel<uint> HidObj3 { get; set; } = new();
        public DataEntryModel<uint> HidObj4 { get; set; } = new();
        public DataEntryModel<uint> HidObj5 { get; set; } = new();
        public DataEntryModel<uint> HidObj6 { get; set; } = new();
        public DataEntryModel<uint> HidObj7 { get; set; } = new();
        public DataEntryModel<uint> HidLgt0 { get; set; } = new();
        public DataEntryModel<uint> HidLgt1 { get; set; } = new();
        public DataEntryModel<uint> HidLgt2 { get; set; } = new();
        public DataEntryModel<uint> HidLgt3 { get; set; } = new();
        public DataEntryModel<uint> FogCol { get; set; } = new();
        public DataEntryModel<float> FogNr { get; set; } = new();
        public DataEntryModel<float> FogFr { get; set; } = new();
        public DataEntryModel<short> LkFlg { get; set; } = new();
        public DataEntryModel<short> LkNo { get; set; } = new();
        public DataEntryModel<short> LkOno { get; set; } = new();
        public DataEntryModel<short> NxtNo { get; set; } = new();
        public DataEntryModel<float> Lx { get; set; } = new();
        public DataEntryModel<float> Ly { get; set; } = new();
        public DataEntryModel<float> Lz { get; set; } = new();
        public DataEntryModel<float> Prm0 { get; set; } = new();
        public DataEntryModel<float> Prm1 { get; set; } = new();
        public DataEntryModel<float> Prm2 { get; set; } = new();
        public DataEntryModel<float> Prm3 { get; set; } = new();
        public DataEntryModel<float> Prm4 { get; set; } = new();
        public DataEntryModel<uint> Recp { get; set; } = new() { IsPointer = true };
    }
}
