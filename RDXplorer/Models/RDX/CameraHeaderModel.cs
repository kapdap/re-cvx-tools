using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    // On-disk layout: CUT_WORK (types.h) — 0x2A8 bytes.
    // Cuttp is a file offset to the CUT_WRK array (see room.c bhSetRoom).
    public class CameraHeaderModel : DataModel<CameraHeaderModelFields>
    {
        public List<CameraBlockModel> Blocks { get; set; } = new();
    }

    public class CameraHeaderModelFields : IFieldsModel
    {
        public DataEntryModel<byte> Flg { get; set; } = new();
        public DataEntryModel<byte> Type { get; set; } = new();
        public DataEntryModel<sbyte> FlrNo { get; set; } = new();
        public DataEntryModel<byte> CTabN { get; set; } = new();
        public DataEntryModel<uint> Cuttp { get; set; } = new() { IsPointer = true };
        public DataEntryModel<float> Cx { get; set; } = new();
        public DataEntryModel<float> Cy { get; set; } = new();
        public DataEntryModel<float> Cz { get; set; } = new();
        public DataEntryModel<float> Cw { get; set; } = new();
        public DataEntryModel<float> Ch { get; set; } = new();
        public DataEntryModel<float> Cd { get; set; } = new();
        public DataEntryModel<byte> Exd { get; set; } = new();
    }
}
