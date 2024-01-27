using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    public class ModelTableModel : DataModel<ModelTableModelFields>
    {
        public List<ModelBlockModel> Blocks { get; set; } = new();
    }

    public class ModelTableModelFields : IFieldsModel
    {
        public DataEntryModel<uint> Pointer { get; set; } = new() { IsPointer = true };
    }
}