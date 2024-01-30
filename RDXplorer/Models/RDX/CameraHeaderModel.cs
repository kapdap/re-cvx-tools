using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    public class CameraHeaderModel : DataModel<CameraHeaderModelFields>
    {
        public List<CameraBlockModel> Blocks { get; set; } = new();
    }

    public class CameraHeaderModelFields : IFieldsModel
    {
        public DataEntryModel<byte> Flag1 { get; set; } = new();
        public DataEntryModel<byte> Flag2 { get; set; } = new();
        public DataEntryModel<byte> Flag3 { get; set; } = new();
        public DataEntryModel<byte> Flag4 { get; set; } = new();
        public DataEntryModel<uint> Pointer { get; set; } = new();
    }
}