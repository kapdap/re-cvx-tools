using System.Collections.Generic;

namespace RDXplorer.Models.RDX
{
    public class TextureTableModel : DataModel<TextureTableModelFields>
    {
        public List<TextureBlockModel> Blocks { get; set; } = new();
    }

    public class TextureTableModelFields : IFieldsModel
    {
        public DataEntryModel<uint> Pointer { get; set; } = new() { IsPointer = true };
    }
}