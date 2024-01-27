using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    public class MotionTableModel : DataModel<MotionTableModelFields>
    {
        public List<MotionBlockModel> Blocks { get; set; } = new();
    }

    public class MotionTableModelFields : IFieldsModel
    {
        public DataEntryModel<uint> Pointer { get; set; } = new() { IsPointer = true };
    }
}