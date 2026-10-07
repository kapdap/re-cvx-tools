namespace RDXplorer.Models.RDX
{
    // On-disk layout: CAM_WRK (types.h) — 0xC4 bytes.
    public class CameraBlockModel : DataModel<CameraModelFields>
    {
        public CameraHeaderModel Header { get; set; }
    }

    public class CameraModelFields : IFieldsModel
    {
        public DataEntryModel<ushort> Flg { get; set; } = new();
        public DataEntryModel<byte> LgtClip { get; set; } = new();
        public DataEntryModel<sbyte> Spd { get; set; } = new();
        public DataEntryModel<float> Px { get; set; } = new();
        public DataEntryModel<float> Py { get; set; } = new();
        public DataEntryModel<float> Pz { get; set; } = new();
        public DataEntryModel<float> Ln { get; set; } = new();
        public DataEntryModel<float> W { get; set; } = new();
        public DataEntryModel<float> H { get; set; } = new();
        public DataEntryModel<float> D { get; set; } = new();
        public DataEntryModel<float> Y0 { get; set; } = new();
        public DataEntryModel<float> Y1 { get; set; } = new();
        public DataEntryModel<float> Y2 { get; set; } = new();
        public DataEntryModel<float> Y3 { get; set; } = new();
        public DataEntryModel<float> AmSpd { get; set; } = new();
        public DataEntryModel<int> Ax { get; set; } = new();
        public DataEntryModel<int> Ay { get; set; } = new();
        public DataEntryModel<int> Az { get; set; } = new();
        public DataEntryModel<int> Lax { get; set; } = new();
        public DataEntryModel<int> Lay { get; set; } = new();
        public DataEntryModel<short> Laz0 { get; set; } = new();
        public DataEntryModel<short> Laz1 { get; set; } = new();
        public DataEntryModel<short> Laz2 { get; set; } = new();
        public DataEntryModel<short> Laz3 { get; set; } = new();
        public DataEntryModel<sbyte> AaSpd { get; set; } = new();
        public DataEntryModel<sbyte> FilNo { get; set; } = new();
        public DataEntryModel<sbyte> FilRt { get; set; } = new();
        public DataEntryModel<sbyte> Reserve { get; set; } = new();
        public DataEntryModel<int> Pers { get; set; } = new();
        public DataEntryModel<uint> HidObj0 { get; set; } = new();
        public DataEntryModel<uint> HidObj1 { get; set; } = new();
        public DataEntryModel<uint> HidObj2 { get; set; } = new();
        public DataEntryModel<uint> HidObj3 { get; set; } = new();
        public DataEntryModel<uint> HidObj4 { get; set; } = new();
        public DataEntryModel<uint> HidObj5 { get; set; } = new();
        public DataEntryModel<uint> HidObj6 { get; set; } = new();
        public DataEntryModel<uint> HidObj7 { get; set; } = new();
        public DataEntryModel<uint> HidObj8 { get; set; } = new();
        public DataEntryModel<uint> HidObj9 { get; set; } = new();
        public DataEntryModel<uint> HidObj10 { get; set; } = new();
        public DataEntryModel<uint> HidObj11 { get; set; } = new();
        public DataEntryModel<uint> HidObj12 { get; set; } = new();
        public DataEntryModel<uint> HidObj13 { get; set; } = new();
        public DataEntryModel<uint> HidObj14 { get; set; } = new();
        public DataEntryModel<uint> HidObj15 { get; set; } = new();
        public DataEntryModel<uint> HidLgt0 { get; set; } = new();
        public DataEntryModel<uint> HidLgt1 { get; set; } = new();
        public DataEntryModel<uint> HidLgt2 { get; set; } = new();
        public DataEntryModel<uint> HidLgt3 { get; set; } = new();
        public DataEntryModel<uint> HidLgt4 { get; set; } = new();
        public DataEntryModel<uint> HidLgt5 { get; set; } = new();
        public DataEntryModel<uint> HidLgt6 { get; set; } = new();
        public DataEntryModel<uint> HidLgt7 { get; set; } = new();
        public DataEntryModel<uint> FogCol { get; set; } = new();
        public DataEntryModel<float> FogNr { get; set; } = new();
        public DataEntryModel<float> FogFr { get; set; } = new();
    }
}
