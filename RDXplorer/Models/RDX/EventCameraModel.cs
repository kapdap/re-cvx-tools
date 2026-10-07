using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    public class EventCameraModel : DataModel<EventCameraModelFields>
    {
        public List<EventCameraBlockModel> Blocks { get; set; } = new();
    }

    public class EventCameraModelFields : IFieldsModel
    {
        public DataEntryModel<ushort> Flg { get; set; } = new();
        public DataEntryModel<ushort> Type { get; set; } = new();
        public DataEntryModel<short> NxtNo { get; set; } = new();
        public DataEntryModel<short> KeyfN { get; set; } = new();
    }
}
